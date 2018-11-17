using ElronAPI.Data.Models;
using ElronAPI.Domain.Classifiers;
using ElronAPI.Domain.Helpers;
using ElronAPI.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ElronAPI.Application.TrainTime.Queries
{
    public class TrainTimesQueryHandler : IRequestHandler<TrainTimesQuery, IEnumerable<TrainTimeModel>>
    {
        private readonly IMemoryCache _memoryCache;
        private readonly PeatusContext _peatusContext;

        public TrainTimesQueryHandler(IMemoryCache memoryCache, PeatusContext peatusContext)
        {
            _memoryCache = memoryCache;
            _peatusContext = peatusContext;
        }

        public async Task<IEnumerable<TrainTimeModel>> Handle(TrainTimesQuery request, CancellationToken cancellationToken)
        {
            string originLowerCase = request.Origin.ToLower();
            string destinationLowerCase = request.Destination.ToLower();

            var trainTimes = await _memoryCache.GetOrCreateAsync(CacheKeyHelper.GetTrainTimesCacheKey(originLowerCase, destinationLowerCase),
                async entry =>
                {
                    var nowEstonian = DateTimeHelper.NowEstonian();

                    entry.AbsoluteExpirationRelativeToNow = DateTimeHelper.TimeUntilMidnight(nowEstonian);

                    var times = await (from originStopTime in _peatusContext.StopTimes
                                       join destinationStopTime in _peatusContext.StopTimes
                                           on originStopTime.TripId equals destinationStopTime.TripId
                                       where (originStopTime.Trip.Route.AgencyId == AgencyType.Elron.Id &&
                                              destinationStopTime.Trip.Route.AgencyId == AgencyType.Elron.Id) &&
                                             (originStopTime.Stop.StopName.ToLower() == originLowerCase &&
                                              destinationStopTime.Stop.StopName.ToLower() == destinationLowerCase) &&
                                             originStopTime.StopSequence < destinationStopTime.StopSequence
                                       select new TrainTimeModel
                                       {
                                           ServiceId = destinationStopTime.Trip.ServiceId,
                                           RouteLongName = originStopTime.Trip.Route.RouteLongName,
                                           RouteShortName = originStopTime.Trip.Route.RouteShortName,
                                           Start = originStopTime.DepartureTime,
                                           End = destinationStopTime.ArrivalTime
                                       }).OrderBy(o => o.Start).ToListAsync(cancellationToken);

                    var calendarList = await (from calendar in _peatusContext.Calendar
                                              join serviceId in times.Select(t => t.ServiceId) on calendar.ServiceId equals serviceId
                                              select calendar).ToListAsync(cancellationToken);

                    for (int i = times.Count - 1; i >= 0; i--)
                    {
                        var time = times[i];
                        var calendar = calendarList.FirstOrDefault(c => c.ServiceId == time.ServiceId);

                        if (calendar == null) continue;

                        if (!StopExistsOnDay(nowEstonian.DayOfWeek, calendar))
                        {
                            times.RemoveAt(i);
                            continue;
                        }

                        if ((nowEstonian.Date < calendar.StartDate || nowEstonian > new DateTime(calendar.EndDate.Year,
                                 calendar.EndDate.Month, calendar.EndDate.Day, 23, 59, 59)))
                        {
                            times.RemoveAt(i);
                        }
                    }

                    return times;
                });

            if (request.All) return trainTimes;

            var timeOfDay = DateTimeHelper.NowEstonian().TimeOfDay;
            return trainTimes.Where(o => TimeSpan.Parse(o.Start) > timeOfDay);
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