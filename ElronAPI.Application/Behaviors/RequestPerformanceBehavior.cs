using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace ElronAPI.Application.Behaviors
{
    public class RequestPerformanceBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly ILogger<TRequest> _logger;
        private readonly Stopwatch _stopwatch;

        public RequestPerformanceBehavior(ILogger<TRequest> logger)
        {
            _logger = logger;
            _stopwatch = new Stopwatch();
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            _stopwatch.Start();

            var response = await next();

            _stopwatch.Stop();

            if (_stopwatch.ElapsedMilliseconds > 800)
            {
                string name = typeof(TRequest).Name;
                _logger.LogWarning("Slow Request: {Name} ({ElapsedMilliseconds} ms) {@Request}", name, _stopwatch.ElapsedMilliseconds, request);
            }

            return response;
        }
    }
}