using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using ElronAPI.Api;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;
using ElronAPI.Api.Data;
using ElronAPI.Api.Models;
using Xunit;

namespace ElronAPI.Tests
{
    public class ApiShould : IDisposable
    {
        public readonly TestServer Server;
        public readonly HttpClient Client;

        public ApiShould()
        {
            Server = new TestServer(new WebHostBuilder()
                .UseEnvironment("Test").UseStartup<Startup>());

            Client = Server.CreateClient();
        }

        [Fact]
        public async Task Return_TrainTimes_BadRequest()
        {
            var response = await Client.GetAsync($"/api/traintimes?origin=&destination=&all=true");

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            string responseString = await response.Content.ReadAsStringAsync();

            var responseObject = JsonConvert.DeserializeObject<JsonErrorResponseModel>(responseString);

            Assert.True(responseObject.Error);
        }

        [Fact]
        public async Task Return_Valid_Account()
        {
            string cardNumber = "92000153082";

            var response = await Client.GetAsync($"/api/elronaccount/{cardNumber}");

            response.EnsureSuccessStatusCode();

            string responseString = await response.Content.ReadAsStringAsync();

            var elronAccount = JsonConvert.DeserializeObject<ElronAccount>(responseString);

            // validate cardnumber
            Assert.Equal(cardNumber, elronAccount.Id);

            Assert.NotNull(elronAccount.Balance);

            Assert.NotEmpty(elronAccount.PeriodTickets);
            Assert.NotEmpty(elronAccount.Transactions);
        }

        [Fact]
        public async Task Return_Error_If_Invalid_Account()
        {
            string cardNumber = "920001653082";

            var response = await Client.GetAsync($"/api/elronaccount/getaccount?id={cardNumber}");

            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);

            string responseString = await response.Content.ReadAsStringAsync();

            var responseObject = JsonConvert.DeserializeObject<JsonErrorResponseModel>(responseString);

            Assert.True(responseObject.Error);
        }

        [Fact]
        public async Task Return_Error_If_No_Account_Specified()
        {
            var response = await Client.GetAsync("/api/elronaccount/");

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            string responseString = await response.Content.ReadAsStringAsync();


            var responseObject = JsonConvert.DeserializeObject<JsonErrorResponseModel>(responseString);

            Assert.True(responseObject.Error);
        }

        public void Dispose()
        {
            Server?.Dispose();
            Client?.Dispose();
        }
    }
}