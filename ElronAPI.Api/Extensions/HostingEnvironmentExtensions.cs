using Microsoft.AspNetCore.Hosting;

namespace ElronAPI.Api.Extensions
{
    public static class HostingEnvironmentExtensions
    {
        public static bool IsTest(this IHostingEnvironment hosting)
        {
            return hosting.IsEnvironment("Test");
        }
    }
}
