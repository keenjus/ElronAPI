using ElronAPI.Data.Models;
using ElronAPI.Domain.Extensions;
using ElronAPI.Domain.Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ElronAPI.Application.TrainTime.Queries
{
    public class TrainTimesLastImportQueryHandler : IRequestHandler<TrainTimesLastImportQuery, DateTime?>
    {
        private readonly IMemoryCache _memoryCache;
        private readonly PeatusContext _peatusContext;

        public TrainTimesLastImportQueryHandler(IMemoryCache memoryCache, PeatusContext peatusContext)
        {
            _memoryCache = memoryCache;
            _peatusContext = peatusContext;
        }

        public async Task<DateTime?> Handle(TrainTimesLastImportQuery request, CancellationToken cancellationToken)
        {
            var lastImportDate = await _memoryCache.GetOrCreateAsync(CacheKeyHelper.GetLastImportDateCacheKey(),
                async entry =>
                {
                    entry.AbsoluteExpiration = DateTime.Now.EndOfDay();

                    var earliestImportLog = await _peatusContext.ImportLogs.OrderByDescending(x => x.ImportDate)
                        .FirstOrDefaultAsync(cancellationToken);

                    return earliestImportLog?.ImportDate;
                });

            return lastImportDate;
        }
    }
}