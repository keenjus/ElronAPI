using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using ElronAPI.Models;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ElronAPI.Controllers
{
    [Route("api/[controller]")]
    public class ElronAccountController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly ApplicationDb _dbContext;
        public ElronAccountController(ApplicationDb dbContext)
        {
            _httpClient = new HttpClient();
            _dbContext = dbContext;
        }

        [HttpGet("{id}", Name = "GetAccount")]
        public async Task<IActionResult> GetById(string id, bool all = false)
        {
            var now = DateTime.Now;
            var exists = _dbContext.ElronAccount
                            .Include(e => e.ActivePeriodTicket)
                            .Include(e => e.PeriodTickets)
                            .Include(e => e.Transactions)
                            .ThenInclude(t => t.Ticket)
                            .FirstOrDefault(e => e.Id == id);
            if (exists != null)
            {
                return new JsonResult(exists);
            }

            var result = await _httpClient.GetAsync($"https://pilet.elron.ee/Account/Login?cardNumber={id}");
            
            // temporary
            result = await _httpClient.GetAsync("https://pilet.elron.ee/Account/Statement?allTransactions=True");
            // if (all)
            // {
            //     result = await _httpClient.GetAsync("https://pilet.elron.ee/Account/Statement?allTransactions=True");
            // }
            result.EnsureSuccessStatusCode();

            string content = await result.Content.ReadAsStringAsync();

            var doc = new HtmlDocument();
            doc.LoadHtml(content);

            var balanceNode = doc.DocumentNode.SelectNodes("//*[@id=\"content\"]/section/section/div[1]/div[2]/fieldset/input");
            if (balanceNode == null || balanceNode.Count != 1)
            {
                return ScrapeError("Failed to find balance node.");
            }
            var valueAttribute = balanceNode[0].Attributes["value"];
            if (valueAttribute == null)
            {
                return ScrapeError("Failed to find balance value attribute.");
            }
            decimal balance;
            if (!decimal.TryParse(valueAttribute.Value.Replace(",", "."), NumberStyles.Number, CultureInfo.InvariantCulture, out balance))
            {
                return ScrapeError("Failed to parse balance.");
            }

            var account = new ElronAccount() { Id = id, Balance = balance, LastCheck = now };

            var transactionsTableNode = doc.DocumentNode.SelectSingleNode("//*[@id=\"content\"]/section/section/section/section/section/div/table/tbody");
            if (transactionsTableNode != null)
            {
                var tableRows = transactionsTableNode.SelectNodes("tr");

                foreach (var tableRow in tableRows)
                {
                    var transaction = new ElronTransaction();

                    var columns = tableRow.SelectNodes("td");

                    var dateNode = columns[0];
                    var ticketNode = columns[1].SelectSingleNode("a");
                    var nameNode = columns[2];
                    var sumNode = columns[3].SelectSingleNode("span");

                    transaction.Date = DateTime.ParseExact(dateNode.InnerText.Trim(), "dd.MM.yyyy HH:mm", CultureInfo.InvariantCulture);
                    transaction.Name = WebUtility.HtmlDecode(nameNode.SelectSingleNode("text()").InnerText).Trim();
                    transaction.Sum = decimal.Parse(sumNode.InnerText.Replace(",", ".").Trim(), NumberStyles.Number, CultureInfo.InvariantCulture);

                    if (ticketNode != null)
                    {
                        var ticketNodeHrefValue = ticketNode.Attributes["href"].Value;
                        var ticketGuid = Guid.Parse(ticketNodeHrefValue.Substring(ticketNodeHrefValue.Length - 36));

                        var ticket = new ElronTicket() { Id = ticketGuid, Number = ticketNode.InnerText.Trim(), Url = new Uri(new Uri("https://pilet.elron.ee/"), ticketNodeHrefValue).AbsoluteUri };
                        transaction.Ticket = ticket;

                        var validityNode = nameNode.SelectSingleNode("div[contains(@class, 'validity')]");
                        if (validityNode != null)
                        {
                            string validityString = validityNode.InnerText.Trim().Substring(10);
                            string[] dateStrings = validityString.Split('-').Select(d => d.Trim()).ToArray();

                            var validFrom = DateTime.ParseExact(dateStrings[0], "dd.MM.yyyy HH:mm", CultureInfo.InvariantCulture);
                            var validTo = DateTime.ParseExact(dateStrings[1], "dd.MM.yyyy HH:mm", CultureInfo.InvariantCulture);

                            // transaction.Name += " " + validityNode.InnerText.Trim();

                            account.PeriodTickets.Add(new ElronPeriodTicket()
                            {
                                Transaction = transaction,
                                ValidFrom = validFrom,
                                ValidTo = validTo
                            });
                        }
                    }
                    account.Transactions.Add(transaction);
                }
                account.ActivePeriodTicket = account.PeriodTickets.OrderByDescending(p => p.ValidFrom).Where(p => p.ValidTo > now).FirstOrDefault();
            }

            _dbContext.ElronAccount.Add(account);
            _dbContext.SaveChanges();

            return new JsonResult(account);
        }

        private IActionResult ScrapeError(string message)
        {
            Response.StatusCode = 500;
            return new JsonResult(new { error = true, message = message });
        }
    }
}