using System;

namespace ElronAPI.Domain.Helpers
{
    public static class DateTimeHelper
    {
        public static DateTime NowEstonian()
        {
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("E. Europe Standard Time"));
        }

        public static TimeSpan TimeUntilMidnight(DateTime dateTime)
        {
            var nextDay = dateTime.Date.AddDays(1);
            var timeSpanUntilMidnight = (nextDay - dateTime);

            return timeSpanUntilMidnight;
        }
    }
}
