using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ElronAPI.Data.Models;
using ElronAPI.Domain.Classifiers;
using ElronAPI.Domain.Extensions;
using ElronAPI.Domain.Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace ElronAPI.Application.TrainTime.Queries
{
    public class TrainStopsQueryHandler : IRequestHandler<TrainStopsQuery, IEnumerable<string>>
    {
        private readonly IMemoryCache _memoryCache;
        private readonly PeatusContext _peatusContext;

        public TrainStopsQueryHandler(IMemoryCache memoryCache, PeatusContext peatusContext)
        {
            _memoryCache = memoryCache;
            _peatusContext = peatusContext;
        }

        public async Task<IEnumerable<string>> Handle(TrainStopsQuery request, CancellationToken cancellationToken)
        {
            var trainStops = await _memoryCache.GetOrCreateAsync(CacheKeyHelper.GetTrainStopsCacheKey(),
                async entry =>
                {
                    entry.AbsoluteExpiration = DateTime.Now.EndOfDay();

                    // this might not be the fastest way to get traintimes
                    return await _peatusContext.Routes.Where(r => r.AgencyId == AgencyType.Elron.Id)
                        .SelectMany(x => x.Trips)
                        .SelectMany(x => x.StopTimes)
                        .Select(x => x.Stop.StopName)
                        .Distinct()
                        .ToListAsync(cancellationToken);
                });

            return trainStops;
        }
    }
}