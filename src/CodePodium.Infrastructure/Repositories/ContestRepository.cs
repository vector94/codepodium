using CodePodium.Core.Interfaces;
using CodePodium.Core.Models;
using CodePodium.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CodePodium.Infrastructure.Repositories;

public class ContestRepository(AppDbContext db) : IContestRepository
{
    public async Task<(IEnumerable<Contest> Items, int TotalCount)> GetPagedAsync(string? platform, string? search, int page, int pageSize)
    {
        var query = db.Contests.AsQueryable();
        if (!string.IsNullOrEmpty(platform))
            query = query.Where(c => c.Platform == platform);
        if (!string.IsNullOrEmpty(search))
            query = query.Where(c => c.Name.ToLower().Contains(search.ToLower()));

        var total = await query.CountAsync();
        var items = await query
            .OrderByDescending(c => c.StartTime)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, total);
    }

    public async Task<IEnumerable<Contest>> GetAllAsync() =>
        await db.Contests.OrderBy(c => c.StartTime).ToListAsync();

    public async Task<IEnumerable<Contest>> GetUpcomingAsync() =>
        await db.Contests
            .Where(c => c.StartTime > DateTime.UtcNow)
            .OrderBy(c => c.StartTime)
            .ToListAsync();

    public async Task<IEnumerable<Contest>> GetByPlatformAsync(string platform) =>
        await db.Contests
            .Where(c => c.Platform == platform)
            .OrderBy(c => c.StartTime)
            .ToListAsync();

    public async Task<Contest?> GetByIdAsync(int id) =>
        await db.Contests.FindAsync(id);

    public async Task<Contest?> GetByExternalIdAsync(string externalId, string platform) =>
        await db.Contests.AsNoTracking().FirstOrDefaultAsync(c => c.ExternalId == externalId && c.Platform == platform);

    public async Task AddAsync(Contest contest) =>
        await db.Contests.AddAsync(contest);

    public async Task SaveChangesAsync() =>
        await db.SaveChangesAsync();

    public async Task<ContestStats> GetStatsAsync()
    {
        var now = DateTime.UtcNow;
        var contests = await db.Contests.ToListAsync();
        return new ContestStats
        {
            Total = contests.Count,
            Upcoming = contests.Count(c => c.StartTime > now),
            Ongoing = contests.Count(c => c.StartTime <= now && c.EndTime > now),
            Finished = contests.Count(c => c.EndTime <= now),
            ByPlatform = contests.GroupBy(c => c.Platform).ToDictionary(g => g.Key, g => g.Count()),
        };
    }
}
