using CodePodium.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace CodePodium.Infrastructure.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Contest> Contests => Set<Contest>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Contest>()
            .HasIndex(c => new { c.ExternalId, c.Platform })
            .IsUnique();
    }
}
