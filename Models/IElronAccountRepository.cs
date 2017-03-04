using System.Collections.Generic;

namespace ElronAPI.Models
{
    public interface IElronAccountRepository
    {
        void Add(ElronAccount account);
        void Remove(string id);
        void Update(ElronAccount account);
        IEnumerable<ElronAccount> GetAll();
        ElronAccount Find(string id);
    }
}