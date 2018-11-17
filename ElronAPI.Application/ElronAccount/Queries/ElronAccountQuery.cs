using ElronAPI.Domain.Models;
using MediatR;

namespace ElronAPI.Application.ElronAccount.Queries
{
    public class ElronAccountQuery : IRequest<ElronAccountModel>
    {
        public string Id { get; set; }
    }
}
