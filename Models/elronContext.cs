using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace ElronAPI.Models
{
    public class elronContext : DbContext
    {
        public elronContext(DbContextOptions<elronContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CachedAccount>(entity =>
            {
                entity.Property(e => e.Data)
                    .IsRequired()
                    .HasColumnType("json");
            });
        }
        
        public DbSet<CachedAccount> CachedAccounts { get; set; }
    }
}