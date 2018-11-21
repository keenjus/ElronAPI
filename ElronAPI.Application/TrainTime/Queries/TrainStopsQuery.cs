using System.Collections.Generic;
using MediatR;

namespace ElronAPI.Application.TrainTime.Queries
{
    public class TrainStopsQuery : IRequest<IEnumerable<string>>
    {

    }
}