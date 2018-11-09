using Hangfire.Dashboard;
using OtpNet;
using System;

namespace ElronAPI.Api.Hangfire
{
    public class TotpAuthorizationFilter : IDashboardAuthorizationFilter
    {
        private const string SessionKey = "HangfireAuthenticated";

        private readonly Totp _totp;

        public TotpAuthorizationFilter(string key)
        {
            if (string.IsNullOrWhiteSpace(key)) throw new InvalidOperationException();

            _totp = new Totp(Base32Encoding.ToBytes(key));
        }

        public bool Authorize(DashboardContext context)
        {
            var httpContext = context.GetHttpContext();

            if (httpContext.Session.TryGetValue(SessionKey, out _))
            {
                return true;
            }

            if (!httpContext.Request.Query.TryGetValue("code", out var c)) return false;

            string code = _totp.ComputeTotp();
            if (code != c) return false;

            httpContext.Session.Set(SessionKey, new byte[] { });
            return true;
        }
    }
}