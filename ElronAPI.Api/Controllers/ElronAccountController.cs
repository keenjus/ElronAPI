using ElronAPI.Application.ElronAccount.Queries;
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
            var query = new ElronAccountQuery() { Id = id };

            return Json(await Mediator.Send(query));
        }
    }
}