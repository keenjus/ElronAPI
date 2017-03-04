using Microsoft.EntityFrameworkCore;

namespace ElronAPI.Models
{
    public class ApplicationDb : DbContext
    {
        public ApplicationDb() : base()
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase();
        }

        public DbSet<ElronAccount> ElronAccounts { get; set; }
    }
}