using System;
using System.Linq;

namespace ElronAPI.Domain.Helpers
{
    public static class DateTimeHelper
    {
        private static string _estonianTimeZoneId;

        private static string EstonianTimeZoneId
        {
            get
            {
                if (_estonianTimeZoneId != null) return _estonianTimeZoneId;

                var timezones = TimeZoneInfo.GetSystemTimeZones()
                    .Where(x => x.Id == "Europe/Tallinn" || x.Id == "E. Europe Standard Time");

                _estonianTimeZoneId = timezones.First().Id;
                return _estonianTimeZoneId;
            }
        }

        public static DateTime NowEstonian()
        {
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById(EstonianTimeZoneId));
        }

        public static TimeSpan TimeUntilMidnight(DateTime dateTime)
        {
            var nextDay = dateTime.Date.AddDays(1);
            var timeSpanUntilMidnight = (nextDay - dateTime);

            return timeSpanUntilMidnight;
        }
    }
}
