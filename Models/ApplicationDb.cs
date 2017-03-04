using Microsoft.EntityFrameworkCore;

namespace ElronAPI.Models
{
    public class ApplicationDb : DbContext
    {
        public ApplicationDb(DbContextOptions<ApplicationDb> options) : base(options)
        {

        }

        public DbSet<ElronAccount> ElronAccounts { get; set; }
    }
}