using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace ElronAPI.Models
{
    public class elronContext : DbContext
    {
        public elronContext(DbContextOptions<elronContext> options) : base(options)
        {

        }
        
        public DbSet<ElronAccount> ElronAccount { get; set; }
    }
}