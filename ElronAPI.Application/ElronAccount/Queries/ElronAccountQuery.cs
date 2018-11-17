using MediatR;

namespace ElronAPI.Application.ElronAccount.Queries
{
    public class ElronAccountQuery : IRequest<int>
    {
        public string Id { get; set; }
    }
}
