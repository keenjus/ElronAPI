using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace ElronAPI.Models
{
    public class ApplicationDb : DbContext
    {
        // public ApplicationDb() : base()
        // {

        // }

        public ApplicationDb(DbContextOptions options) : base(options)
        {

        }

        // protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        // {
        //     optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=Test;Trusted_Connection=True;");
        // }

        public DbSet<ElronAccount> ElronAccounts { get; set; }
    }
}