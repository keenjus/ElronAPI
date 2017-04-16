using System;
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
            var now = DateTime.Now.TimeOfDay;

            var originStopTimes = _dbContext.StopTimes.Where(st => st.Stop.StopName.ToLower() == origin && st.Trip.Route.AgencyId == 82);
            var destinationStopTimes = _dbContext.StopTimes.Where(st => st.Stop.StopName.ToLower() == destination && st.Trip.Route.AgencyId == 82);

            var times = (from originStopTime in originStopTimes
                         join destinationStopTime in destinationStopTimes
                         on originStopTime.TripId equals destinationStopTime.TripId
                         where originStopTime.StopSequence < destinationStopTime.StopSequence
                         select new
                         {
                             Start = originStopTime.DepartureTime,
                             End = destinationStopTime.ArrivalTime
                         }).OrderBy(o => o.Start).ToList();

            if(!all) times.RemoveAll(o => TimeSpan.Parse(o.Start) < now);

            return times;
        }
    }
}