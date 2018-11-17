using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ElronAPI.Domain.Models;
using MediatR;

namespace ElronAPI.Application.TrainTime.Queries
{
    public class TrainTimesQueryHandler : IRequestHandler<TrainTimesQuery, IEnumerable<TrainTimeModel>>
    {
        public Task<IEnumerable<TrainTimeModel>> Handle(TrainTimesQuery request, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}