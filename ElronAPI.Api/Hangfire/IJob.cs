using System.Threading.Tasks;

namespace ElronAPI.Api.Hangfire
{
    public interface IJob
    {
        Task WorkAsync();
    }
}