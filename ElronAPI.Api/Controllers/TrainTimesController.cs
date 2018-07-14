using ElronAPI.Api.Data;
using ElronAPI.Api.Models;
using ElronAPI.Domain.Classifiers;
using ElronAPI.Domain.Extensions;
using ElronAPI.Domain.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElronAPI.Api.Controllers
{
    [Route("api/[controller]")]
    public class TrainTimesController : Controller
    {
        private readonly PeatusContext _dbContext;
        private readonly IMemoryCache _memoryCache;

        public TrainTimesController(PeatusContext dbContext, IMemoryCache memoryCache)
        {
            _dbContext = dbContext;
            _memoryCache = memoryCache;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string origin, string destination, bool all = false)
        {
            if (!string.IsNullOrWhiteSpace(origin) && !string.IsNullOrWhiteSpace(destination))
                return Json(await GetTrainTimes(origin, destination, all));

            Response.StatusCode = 400;
            return Json(new JsonErrorResponseModel { Error = true, Message = "Missing parameters" });
        }

        [HttpGet("stops")]
        public async Task<IActionResult> Stops()
        {
            var trainStops = await _memoryCache.GetOrCreateAsync(CacheKeyHelper.GetTrainStopsCacheKey(), async entry =>
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

        private async Task<List<TrainTimeModel>> GetTrainTimes(string origin, string destination, bool all = false)
        {
            string originLowerCase = origin.ToLower();
            string destinationLowerCase = destination.ToLower();

            var traintimes =  await _memoryCache.GetOrCreateAsync(CacheKeyHelper.GetTrainTimesCacheKey(origin, destination), async entry =>
            {
                entry.AbsoluteExpiration = DateTime.Now.EndOfDay();

                var times = await (from originStopTime in _dbContext.StopTimes
                                   join destinationStopTime in _dbContext.StopTimes
                                   on originStopTime.TripId equals destinationStopTime.TripId
                                   where (originStopTime.Trip.Route.AgencyId == AgencyType.Elron.Id && destinationStopTime.Trip.Route.AgencyId == AgencyType.Elron.Id) &&
                                         (originStopTime.Stop.StopName.ToLower() == originLowerCase && destinationStopTime.Stop.StopName.ToLower() == destinationLowerCase) &&
                                         originStopTime.StopSequence < destinationStopTime.StopSequence
                                   select new TrainTimeModel
                                   {
                                       ServiceId = destinationStopTime.Trip.ServiceId,
                                       RouteLongName = originStopTime.Trip.Route.RouteLongName,
                                       RouteShortName = originStopTime.Trip.Route.RouteShortName,
                                       Start = originStopTime.DepartureTime,
                                       End = destinationStopTime.ArrivalTime
                                   }).OrderBy(o => o.Start).ToListAsync();

                var calendarList = await (from calendar in _dbContext.Calendar
                                          join serviceId in times.Select(t => t.ServiceId) on calendar.ServiceId equals serviceId
                                          select calendar).ToListAsync();

                var now = DateTime.Now;

                for (int i = times.Count - 1; i >= 0; i--)
                {
                    var time = times[i];
                    var calendar = calendarList.FirstOrDefault(c => c.ServiceId == time.ServiceId);

                    if (calendar == null) continue;

                    if (!StopExistsOnDay(now.DayOfWeek, calendar))
                    {
                        times.RemoveAt(i);
                        continue;
                    }

                    if ((now.Date < calendar.StartDate || now > new DateTime(calendar.EndDate.Year, calendar.EndDate.Month, calendar.EndDate.Day, 23, 59, 59)))
                    {
                        times.RemoveAt(i);
                    }
                }

                return times;
            });

            if (all == false)
            {
                var timeOfDay = DateTime.Now.TimeOfDay;
                return traintimes.Where(o => TimeSpan.Parse(o.Start) > timeOfDay).ToList();
            }

            return traintimes;
        }

        private static bool StopExistsOnDay(DayOfWeek day, Calendar calendar)
        {
            switch (day)
            {
                case DayOfWeek.Monday:
                    return calendar.Monday;
                case DayOfWeek.Tuesday:
                    return calendar.Tuesday;
                case DayOfWeek.Wednesday:
                    return calendar.Wednesday;
                case DayOfWeek.Thursday:
                    return calendar.Thursday;
                case DayOfWeek.Friday:
                    return calendar.Friday;
                case DayOfWeek.Saturday:
                    return calendar.Saturday;
                case DayOfWeek.Sunday:
                    return calendar.Sunday;
                default:
                    return false;
            }
        }
    }
}