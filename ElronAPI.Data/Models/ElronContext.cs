using Microsoft.EntityFrameworkCore;

namespace ElronAPI.Data.Models
{
    public class ElronContext : DbContext
    {
        public ElronContext(DbContextOptions<ElronContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CachedResponse>(entity =>
            {
                entity.Property(e => e.Data)
                    .IsRequired()
                    .HasColumnType("jsonb");
                entity.Property(e => e.ExpireTime)
                    .IsRequired()
                    .HasColumnType("timestamp");
            });
        }
        
        public DbSet<CachedResponse> CachedResponses { get; set; }
    }
}