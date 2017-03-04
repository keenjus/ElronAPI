using System.Collections.Generic;
using System.Linq;

namespace ElronAPI.Models
{
    public class ElronAccountRepository : IElronAccountRepository
    {
        private readonly ApplicationDb _context;

        public ElronAccountRepository(ApplicationDb context)
        {
            _context = context;
        }

        public void Add(ElronAccount account)
        {
            _context.ElronAccounts.Add(account);
            _context.SaveChanges();
        }

        public void Remove(string number)
        {
            var entity = _context.ElronAccounts.First(i => i.Number == number);
            _context.ElronAccounts.Remove(entity);
            _context.SaveChanges();
        }

        public void Update(ElronAccount account)
        {
            _context.ElronAccounts.Update(account);
            _context.SaveChanges();
        }

        public IEnumerable<ElronAccount> GetAll()
        {
            return _context.ElronAccounts.ToList();
        }

        public ElronAccount Find(string number)
        {
            return _context.ElronAccounts.FirstOrDefault(i => i.Number == number);
        }
    }
}