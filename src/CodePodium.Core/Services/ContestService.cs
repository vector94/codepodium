using CodePodium.Core.Interfaces;
using CodePodium.Core.Models;

namespace CodePodium.Core.Services;

public class ContestService(IContestRepository contestRepository, IEnumerable<IContestFetcher> fetchers)
{
    public async Task SyncContestsAsync()
    {
        foreach (var fetcher in fetchers)
        {
            var contests = await fetcher.FetchContestsAsync();
            foreach (var contest in contests)
            {
                var existing = await contestRepository.GetByExternalIdAsync(contest.ExternalId, contest.Platform);
                if (existing is null)
                    await contestRepository.AddAsync(contest);
            }
        }
        await contestRepository.SaveChangesAsync();
    }

    public Task<(IEnumerable<Contest> Items, int TotalCount)> GetPagedContestsAsync(string? platform, string? search, int page, int pageSize) =>
        contestRepository.GetPagedAsync(platform, search, page, pageSize);

    public Task<IEnumerable<Contest>> GetAllContestsAsync() =>
        contestRepository.GetAllAsync();

    public Task<IEnumerable<Contest>> GetUpcomingContestsAsync() =>
        contestRepository.GetUpcomingAsync();

    public Task<IEnumerable<Contest>> GetContestsByPlatformAsync(string platform) =>
        contestRepository.GetByPlatformAsync(platform);

    public Task<Contest?> GetContestByIdAsync(int id) =>
        contestRepository.GetByIdAsync(id);

    public Task<ContestStats> GetStatsAsync() =>
        contestRepository.GetStatsAsync();
}
