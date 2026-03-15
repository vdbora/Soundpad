using Microsoft.EntityFrameworkCore;
using SoundboardApp.Models;

namespace SoundboardApp.Data
{
    /// <summary>
    /// Database context for sound storage
    /// </summary>
    public class SoundDbContext : DbContext
    {
        public DbSet<Sound> Sounds { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=soundboard.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Sound>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.FilePath).IsRequired().HasMaxLength(500);
                entity.Property(e => e.Category).HasMaxLength(100);
                entity.Property(e => e.DateAdded).HasDefaultValueSql("datetime('now')");
            });
        }
    }
}
