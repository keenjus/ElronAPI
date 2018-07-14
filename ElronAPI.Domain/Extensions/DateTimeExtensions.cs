using System;

namespace ElronAPI.Domain.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime EndOfDay(this DateTime dt)
        {
            return dt.Date.AddTicks(-1).AddDays(1);
        }
    }
}
