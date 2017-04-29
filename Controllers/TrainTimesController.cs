using System;
using System.Collections.Generic;
using System.Linq;
using ElronAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ElronAPI.Controllers
{
    [Route("api/[controller]")]
    public class TrainTimesController : Controller
    {
        private peatusContext _dbContext;

        public TrainTimesController(peatusContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IActionResult Index(string origin, string destination, bool all = false)
        {
            if (string.IsNullOrWhiteSpace(origin) || string.IsNullOrWhiteSpace(destination))
            {
                Response.StatusCode = 403;
                return new JsonResult(new { error = true, message = "Missing parameters" });
            }
            return new JsonResult(GetTrainTimes(origin.ToLower(), destination.ToLower(), all));
        }

        private object GetTrainTimes(string origin, string destination, bool all = false)
        {
            var now = DateTime.Now;
            var timeOfDay = now.TimeOfDay;

            var times = (from originStopTime in _dbContext.StopTimes
                         join destinationStopTime in _dbContext.StopTimes
                         on originStopTime.TripId equals destinationStopTime.TripId
                         where (originStopTime.Trip.Route.AgencyId == 82 && destinationStopTime.Trip.Route.AgencyId == 82) &&
                               (originStopTime.Stop.StopName.ToLower() == origin && destinationStopTime.Stop.StopName.ToLower() == destination) &&
                               originStopTime.StopSequence < destinationStopTime.StopSequence
                         select new
                         {
                             ServiceId = destinationStopTime.Trip.ServiceId,
                             RouteLongName = originStopTime.Trip.Route.RouteLongName,
                             RouteShortName = originStopTime.Trip.Route.RouteShortName,
                             Start = originStopTime.DepartureTime,
                             End = destinationStopTime.ArrivalTime
                         }).OrderBy(o => o.Start).ToList();

            if (!all) times.RemoveAll(o => TimeSpan.Parse(o.Start) < timeOfDay);

            var calendarList = (from calendar in _dbContext.Calendar
                                join serviceId in times.Select(t => t.ServiceId) on calendar.ServiceId equals serviceId
                                select calendar).ToList();

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

                if ((now < calendar.StartDate || now > calendar.EndDate))
                {
                    times.RemoveAt(i);
                }
            }

            return times;
        }

        private bool StopExistsOnDay(DayOfWeek day, Calendar calendar)
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