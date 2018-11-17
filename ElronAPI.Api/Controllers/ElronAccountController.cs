using ElronAPI.Api.Data;
using ElronAPI.Api.Models;
using ElronAPI.Application.ElronAccount.Queries;
using ElronAPI.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ElronAPI.Api.Controllers
{
    [Route("api/[controller]")]
    public class ElronAccountController : BaseController
    {
        [HttpGet]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                Response.StatusCode = 400;
                return Json(new JsonErrorResponseModel { Error = true, Message = "Card number not specified or is null" });
            }

            try
            {
                var query = new ElronAccountQuery() { Id = id };
                return Json(await Mediator.Send(query));
            }
            catch (ScrapeException ex)
            {
                return ScrapeError(ex.Message);
            }
        }

        private IActionResult ScrapeError(string message)
        {
            Response.StatusCode = 500;
            return Json(new JsonErrorResponseModel { Error = true, Message = message });
        }
    }
}