using ElronAPI.Application.TrainTime.Queries;
using ElronAPI.Data.Models;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Threading.Tasks;

namespace ElronAPI.Api.Controllers
{
    [Route("api/[controller]")]
    public class TrainTimesController : BaseController
    {
        private readonly PeatusContext _dbContext;
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<TrainTimesController> _logger;

        public TrainTimesController(PeatusContext dbContext, IMemoryCache memoryCache,
            ILogger<TrainTimesController> logger)
        {
            _dbContext = dbContext;
            _memoryCache = memoryCache;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string origin, string destination, bool all = false)
        {
            try
            {
                var query = new TrainTimesQuery() { Origin = origin, Destination = destination, All = all };
                var response = await Mediator.Send(query);

                return Json(response);
            }
            catch (ValidationException ex)
            {
                Response.StatusCode = (int)HttpStatusCode.UnprocessableEntity;
                return Content(ex.Message, "text/plain");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception");

                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Content(ex.Message, "text/plain");
            }
        }

        [HttpGet("lastimport")]
        public async Task<IActionResult> LastImport()
        {
            try
            {
                var response = await Mediator.Send(new TrainTimesLastImportQuery());

                return Json(new
                {
                    lastImportDate = response
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception");

                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Content(ex.Message, "text/plain");
            }
        }

        [HttpGet("stops")]
        public async Task<IActionResult> Stops()
        {
            try
            {
                var response = await Mediator.Send(new TrainStopsQuery());

                return Json(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception");

                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Content(ex.Message, "text/plain");
            }
        }
    }
}