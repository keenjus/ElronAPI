using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace ElronAPI.Application.ElronAccount.Queries
{
    public class ElronAccountQueryHandler : IRequestHandler<ElronAccountQuery, int>
    {
        public async Task<int> Handle(ElronAccountQuery request, CancellationToken cancellationToken)
        {
            return 1;
        }
    }
}