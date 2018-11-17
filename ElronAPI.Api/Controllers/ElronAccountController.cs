using AngleSharp.Parser.Html;
using ElronAPI.Api.Data;
using ElronAPI.Api.Models;
using ElronAPI.Domain.Exceptions;
using ElronAPI.Domain.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using ElronAPI.Application.ElronAccount.Queries;
using MediatR;

namespace ElronAPI.Api.Controllers
{
    [Route("api/[controller]")]
    public class ElronAccountController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _memoryCache;
        private readonly IMediator _mediator;

        private static readonly Uri ElronBaseUri = new Uri("https://pilet.elron.ee/");

        public ElronAccountController(IMemoryCache memoryCache, IMediator mediator)
        {
            _httpClient = new HttpClient();
            _memoryCache = memoryCache;
            _mediator = mediator;
        }

        [HttpGet]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                Response.StatusCode = 400;
                return Json(new JsonErrorResponseModel { Error = true, Message = "Cardnumber not specified or is null" });
            }

            try
            {
                var accountData = await _memoryCache.GetOrCreateAsync(CacheKeyHelper.GetAccountCacheKey(id), async entry =>
                {
                    // cache the account object for 15minutes
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15);

                    
                    await _httpClient.GetAsync(new Uri(ElronBaseUri, $"/Account/Login?cardNumber={id}"));
                    var result = await _httpClient.GetAsync(new Uri(ElronBaseUri, "/Account/Statement?allTransactions=True"));

                    result.EnsureSuccessStatusCode();

                    var parser = new HtmlParser();
                    var content = parser.Parse(await result.Content.ReadAsStringAsync()).QuerySelector(".content");

                    var balanceNode = content.QuerySelector("fieldset input.currency");
                    if (balanceNode == null)
                    {
                        throw new ScrapeException("Failed to find balance node.");
                    }

                    if (!decimal.TryParse(balanceNode.Attributes["value"].Value.Replace(",", "."), NumberStyles.Number, CultureInfo.InvariantCulture, out decimal balance))
                    {
                        throw new ScrapeException("Failed to parse balance.");
                    }

                    var account = new ElronAccountModel { Id = id, Balance = balance, LastCheck = DateTime.Now };

                    var transactionsTableNode = content.QuerySelector("table > tbody");
                    if (transactionsTableNode == null) return account;

                    var tableRows = transactionsTableNode.QuerySelectorAll("tr");

                    foreach (var tableRow in tableRows)
                    {
                        var transaction = new ElronAccountModel.Transaction();

                        var columns = tableRow.QuerySelectorAll("td");

                        var dateNode = columns[0];
                        var nameNode = columns[2];
                        var sumNode = columns[3].QuerySelector("span");

                        transaction.Date = DateTime.ParseExact(dateNode.TextContent.Trim(), "dd.MM.yyyy HH:mm", CultureInfo.InvariantCulture);
                        transaction.Sum = decimal.Parse(sumNode.TextContent.Replace(",", ".").Trim(), NumberStyles.Number, CultureInfo.InvariantCulture);

                        // This needs a better solution
                        transaction.Name = nameNode.TextContent.Trim().Split('\t')[0].Trim();

                        var ticketNode = columns[1].QuerySelector("a");
                        if (ticketNode == null)
                        {
                            goto AddTransaction;
                        }

                        string tickerNodeHref = ticketNode.Attributes["href"].Value;

                        var ticketGuid = Guid.Parse(tickerNodeHref.Substring(tickerNodeHref.Length - 36));

                        var ticket = new ElronAccountModel.Ticket()
                        {
                            Id = ticketGuid,
                            Number = ticketNode.TextContent.Trim(),
                            Url = new Uri(ElronBaseUri, tickerNodeHref).AbsoluteUri
                        };

                        transaction.Ticket = ticket;

                        var validityNode = nameNode.QuerySelector("div.validity");
                        if (validityNode == null)
                        {
                            goto AddTransaction;
                        }

                        string validityString = validityNode.TextContent.Trim().Substring(10);
                        var dateStrings = validityString.Split('-').Select(d => d.Trim()).ToArray();

                        var validFrom = DateTime.ParseExact(dateStrings[0], "dd.MM.yyyy HH:mm", CultureInfo.InvariantCulture);
                        var validTo = DateTime.ParseExact(dateStrings[1], "dd.MM.yyyy HH:mm", CultureInfo.InvariantCulture);

                        var periodTicket = new ElronAccountModel.PeriodTicket()
                        {
                            ValidFrom = validFrom,
                            ValidTo = validTo
                        };

                        transaction.PeriodTicket = periodTicket;

                        AddTransaction:
                        account.Transactions.Add(transaction);
                    }

                    return account;
                });

                return Json(SortAccountTransactions(accountData));

            }
            catch (ScrapeException ex)
            {
                return ScrapeError(ex.Message);
            }
        }

        private static ElronAccountModel SortAccountTransactions(ElronAccountModel account)
        {
            if (account?.Transactions != null)
            {
                account.Transactions = account.Transactions.OrderByDescending(t => t.Date).ToList();
            }
            return account;
        }

        private IActionResult ScrapeError(string message)
        {
            Response.StatusCode = 500;
            return Json(new JsonErrorResponseModel { Error = true, Message = message });
        }
    }
}