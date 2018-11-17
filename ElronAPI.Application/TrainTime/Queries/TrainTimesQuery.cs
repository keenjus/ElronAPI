using System.Collections.Generic;
using ElronAPI.Domain.Models;
using MediatR;

namespace ElronAPI.Application.TrainTime.Queries
{
    public class TrainTimesQuery : IRequest<IEnumerable<TrainTimeModel>>
    {
        public string Origin { get; set; }
        public string Destination { get; set; }
    }
}