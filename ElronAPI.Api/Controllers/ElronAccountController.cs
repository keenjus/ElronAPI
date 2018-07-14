using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using ElronAPI.Api.Data;
using ElronAPI.Api.Models;
using ElronAPI.Domain.Exceptions;
using ElronAPI.Domain.Helpers;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace ElronAPI.Api.Controllers
{
    [Route("api/[controller]")]
    public class ElronAccountController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _memoryCache;

        public ElronAccountController(IMemoryCache memoryCache)
        {
            _httpClient = new HttpClient();
            _memoryCache = memoryCache;
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

                    await _httpClient.GetAsync($"https://pilet.elron.ee/Account/Login?cardNumber={id}");
                    var result = await _httpClient.GetAsync("https://pilet.elron.ee/Account/Statement?allTransactions=True");

                    result.EnsureSuccessStatusCode();

                    string content = await result.Content.ReadAsStringAsync();

                    var doc = new HtmlDocument();
                    doc.LoadHtml(content);

                    var balanceNode = doc.DocumentNode.SelectNodes("//*[@id=\"content\"]/section/section/div[1]/div[2]/fieldset/input");
                    if (balanceNode == null || balanceNode.Count != 1)
                    {
                        throw new ScrapeException("Failed to find balance node.");
                    }

                    var valueAttribute = balanceNode[0].Attributes["value"];
                    if (valueAttribute == null)
                    {
                        throw new ScrapeException("Failed to find balance value attribute.");
                    }

                    if (!decimal.TryParse(valueAttribute.Value.Replace(",", "."), NumberStyles.Number,
                        CultureInfo.InvariantCulture, out decimal balance))
                    {
                        throw new ScrapeException("Failed to parse balance.");
                    }

                    var account = new ElronAccount {Id = id, Balance = balance, LastCheck = DateTime.Now};

                    var transactionsTableNode = doc.DocumentNode.SelectSingleNode("//*[@id=\"content\"]/section/section/section/section/section/div/table/tbody");

                    if (transactionsTableNode == null) return account;

                    var tableRows = transactionsTableNode.SelectNodes("tr");

                    foreach (var tableRow in tableRows)
                    {
                        var transaction = new ElronTransaction();

                        var columns = tableRow.SelectNodes("td");

                        var dateNode = columns[0];
                        var ticketNode = columns[1].SelectSingleNode("a");
                        var nameNode = columns[2];
                        var sumNode = columns[3].SelectSingleNode("span");

                        transaction.Date = DateTime.ParseExact(dateNode.InnerText.Trim(), "dd.MM.yyyy HH:mm",
                            CultureInfo.InvariantCulture);
                        transaction.Name = WebUtility.HtmlDecode(nameNode.SelectSingleNode("text()").InnerText)
                            .Trim();
                        transaction.Sum = decimal.Parse(sumNode.InnerText.Replace(",", ".").Trim(),
                            NumberStyles.Number, CultureInfo.InvariantCulture);

                        if (ticketNode != null)
                        {
                            string ticketNodeHrefValue = ticketNode.Attributes["href"].Value;
                            var ticketGuid =
                                Guid.Parse(ticketNodeHrefValue.Substring(ticketNodeHrefValue.Length - 36));

                            var ticket = new ElronTicket
                            {
                                Id = ticketGuid,
                                Number = ticketNode.InnerText.Trim(),
                                Url = new Uri(new Uri("https://pilet.elron.ee/"), ticketNodeHrefValue).AbsoluteUri
                            };
                            transaction.Ticket = ticket;

                            var validityNode = nameNode.SelectSingleNode("div[contains(@class, 'validity')]");
                            if (validityNode != null)
                            {
                                string validityString = validityNode.InnerText.Trim().Substring(10);
                                string[] dateStrings = validityString.Split('-').Select(d => d.Trim()).ToArray();

                                var validFrom = DateTime.ParseExact(dateStrings[0], "dd.MM.yyyy HH:mm",
                                    CultureInfo.InvariantCulture);
                                var validTo = DateTime.ParseExact(dateStrings[1], "dd.MM.yyyy HH:mm",
                                    CultureInfo.InvariantCulture);

                                var periodTicket = new ElronPeriodTicket()
                                {
                                    ValidFrom = validFrom,
                                    ValidTo = validTo
                                };

                                transaction.PeriodTicket = periodTicket;
                                account.PeriodTickets.Add(periodTicket);
                            }
                        }

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

        public ElronAccount SortAccountTransactions(ElronAccount account)
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