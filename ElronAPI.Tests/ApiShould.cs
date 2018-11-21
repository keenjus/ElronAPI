using ElronAPI.Api;
using ElronAPI.Api.Models;
using ElronAPI.Domain.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace ElronAPI.Tests
{
    public class ApiShould : IDisposable
    {
        private readonly TestServer _server;
        private readonly HttpClient _client;

        public ApiShould()
        {
            _server = new TestServer(new WebHostBuilder()
                .UseEnvironment("Test").UseStartup<Startup>());

            _client = _server.CreateClient();
        }

        [Fact]
        public async Task Return_TrainTimes_BadRequest()
        {
            var response = await _client.GetAsync($"/api/traintimes?origin=&destination=&all=true");

            Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        }

        [Fact]
        public async Task Return_Valid_Account()
        {
            string cardNumber = "92000153082";

            var response = await _client.GetAsync($"/api/elronaccount/{cardNumber}");

            response.EnsureSuccessStatusCode();

            string responseString = await response.Content.ReadAsStringAsync();

            var elronAccount = JsonConvert.DeserializeObject<ElronAccountModel>(responseString);

            // validate cardnumber
            Assert.Equal(cardNumber, elronAccount.Id);

            Assert.NotNull(elronAccount.Balance);

            Assert.NotEmpty(elronAccount.PeriodTickets);
            Assert.NotEmpty(elronAccount.Transactions);
        }

        [Fact]
        public async Task Return_UnprocessableEntity_If_Account_Number_Too_Short()
        {
            string cardNumber = "9200015300";

            var response = await _client.GetAsync($"/api/elronaccount/{cardNumber}");

            Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        }

        [Fact]
        public async Task Return_UnprocessableEntity_If_Account_Number_Too_Long()
        {
            string cardNumber = "92000165308257413354";

            var response = await _client.GetAsync($"/api/elronaccount/{cardNumber}");

            Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        }

        [Fact]
        public async Task Return_UnprocessableEntity_If_No_Account_Specified()
        {
            var response = await _client.GetAsync("/api/elronaccount/");

            Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        }

        [Fact]
        public async Task Return_NotFound_If_Invalid_Account()
        {
            string cardNumber = "1212121212121";

            var response = await _client.GetAsync($"/api/elronaccount/{cardNumber}");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        public void Dispose()
        {
            _server?.Dispose();
            _client?.Dispose();
        }
    }
}
