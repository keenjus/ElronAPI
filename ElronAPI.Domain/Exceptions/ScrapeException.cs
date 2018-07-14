using System;

namespace ElronAPI.Domain.Exceptions
{
    public class ScrapeException : Exception
    {
        public ScrapeException() : base("Error occured when scraping response")
        {

        }

        public ScrapeException(string message) : base(message)
        {

        }
    }
}
