using System;
using MediatR;

namespace ElronAPI.Application.TrainTime.Queries
{
    public class TrainTimesLastImportQuery : IRequest<DateTime?>
    {

    }
}