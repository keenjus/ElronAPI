using ElronAPI.Application.TrainTime.Queries;
using ElronAPI.Data.Models;
using ElronAPI.Domain.Classifiers;
using ElronAPI.Domain.Extensions;
using ElronAPI.Domain.Helpers;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
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
            var lastImportDate = await _memoryCache.GetOrCreateAsync(CacheKeyHelper.GetLastImportDateCacheKey(),
                async entry =>
                {
                    entry.AbsoluteExpiration = DateTime.Now.EndOfDay();
                    return (await _dbContext.ImportLogs.OrderByDescending(x => x.ImportDate).FirstOrDefaultAsync())?.ImportDate;
                });

            return Json(new
            {
                lastImportDate
            });
        }

        [HttpGet("stops")]
        public async Task<IActionResult> Stops()
        {
            var trainStops = await _memoryCache.GetOrCreateAsync(CacheKeyHelper.GetTrainStopsCacheKey(),
                async entry =>
                {
                    entry.AbsoluteExpiration = DateTime.Now.EndOfDay();

                    // this might not be the fastest way to get traintimes
                    return await _dbContext.Routes.Where(r => r.AgencyId == AgencyType.Elron.Id)
                        .SelectMany(x => x.Trips)
                        .SelectMany(x => x.StopTimes)
                        .Select(x => x.Stop.StopName)
                        .Distinct()
                        .ToListAsync();
                });

            return Json(trainStops);
        }
    }
}