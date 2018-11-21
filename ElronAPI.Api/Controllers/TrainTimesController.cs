using ElronAPI.Application.TrainTime.Queries;
using ElronAPI.Data.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ElronAPI.Api.Controllers
{
    [Route("api/[controller]")]
    public class TrainTimesController : BaseController
    {
        [HttpGet]
        public async Task<IActionResult> Index(string origin, string destination, bool all = false)
        {
            var query = new TrainTimesQuery() { Origin = origin, Destination = destination, All = all };
            var response = await Mediator.Send(query);

            return Json(response);
        }

        [HttpGet("lastimport")]
        public async Task<IActionResult> LastImport()
        {
            var response = await Mediator.Send(new TrainTimesLastImportQuery());

            return Json(new
            {
                lastImportDate = response
            });
        }

        [HttpGet("stops")]
        public async Task<IActionResult> Stops()
        {
            var response = await Mediator.Send(new TrainStopsQuery());

            return Json(response);
        }
    }
}