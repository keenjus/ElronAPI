using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using ElronAPI.Models;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;

namespace ElronAPI.Controllers
{
    [Route("api/[controller]")]
    public class ElronAccountController : Controller
    {
        private readonly ApplicationDb _dbContext;
        private readonly HttpClient _httpClient;
        public ElronAccountController()
        {
            _dbContext = new ApplicationDb();
            _httpClient = new HttpClient();
        }

        [HttpGet("{id}", Name = "GetAccount")]
        public async Task<IActionResult> GetById(string id)
        {
            var exists = _dbContext.ElronAccounts.FirstOrDefault(e => e.Id == id);
            if (exists != null)
            {
                if ((DateTime.UtcNow - exists.LastCheck).Minutes < 15)
                {
                    return new ObjectResult(exists);
                }
                _dbContext.ElronAccounts.Remove(exists);
                _dbContext.SaveChanges();
            }

            var result = await _httpClient.GetAsync($"https://pilet.elron.ee/Account/Login?cardNumber={id}");
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
            if (!decimal.TryParse(valueAttribute.Value.Replace(",", "."), out balance))
            {
                return ScrapeError("Failed to parse balance.");
            }

            var account = new ElronAccount() { Id = id, Balance = balance, LastCheck = DateTime.UtcNow };
            _dbContext.ElronAccounts.Add(account);
            _dbContext.SaveChanges();

            return new ObjectResult(account);
        }

        private IActionResult ScrapeError(string message)
        {
            Response.StatusCode = 500;
            return new JsonResult(new { error = true, message = message });
        }
    }
}