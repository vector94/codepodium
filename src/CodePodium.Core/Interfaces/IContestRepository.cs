using CodePodium.Core.Models;

namespace CodePodium.Core.Interfaces;

public interface IContestRepository
{
    Task<(IEnumerable<Contest> Items, int TotalCount)> GetPagedAsync(string? platform, string? search, int page, int pageSize);
    Task<IEnumerable<Contest>> GetAllAsync();
    Task<IEnumerable<Contest>> GetUpcomingAsync();
    Task<IEnumerable<Contest>> GetByPlatformAsync(string platform);
    Task<Contest?> GetByIdAsync(int id);
    Task<Contest?> GetByExternalIdAsync(string externalId, string platform);
    Task AddAsync(Contest contest);
    Task SaveChangesAsync();
    Task<ContestStats> GetStatsAsync();
}
