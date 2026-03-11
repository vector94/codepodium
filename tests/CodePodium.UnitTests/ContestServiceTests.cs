using CodePodium.Core.Interfaces;
using CodePodium.Core.Models;
using CodePodium.Core.Services;
using NSubstitute;

namespace CodePodium.UnitTests;

public class ContestServiceTests
{
    private readonly IContestRepository _repo = Substitute.For<IContestRepository>();
    private readonly IContestFetcher _fetcher = Substitute.For<IContestFetcher>();
    private readonly ContestService _sut;

    public ContestServiceTests()
    {
        _fetcher.Platform.Returns("Codeforces");
        _sut = new ContestService(_repo, [_fetcher]);
    }

    [Fact]
    public async Task SyncContestsAsync_SkipsDuplicates()
    {
        var contest = new Contest { ExternalId = "1", Name = "Round 1", Platform = "Codeforces", StartTime = DateTime.UtcNow.AddHours(1), EndTime = DateTime.UtcNow.AddHours(3) };
        _fetcher.FetchContestsAsync().Returns([contest]);
        _repo.GetByExternalIdAsync("1", "Codeforces").Returns(contest);

        await _sut.SyncContestsAsync();

        await _repo.DidNotReceive().AddAsync(Arg.Any<Contest>());
    }

    [Fact]
    public async Task SyncContestsAsync_AddsNewContests()
    {
        var contest = new Contest { ExternalId = "99", Name = "New Round", Platform = "Codeforces", StartTime = DateTime.UtcNow.AddHours(1), EndTime = DateTime.UtcNow.AddHours(3) };
        _fetcher.FetchContestsAsync().Returns([contest]);
        _repo.GetByExternalIdAsync("99", "Codeforces").Returns((Contest?)null);

        await _sut.SyncContestsAsync();

        await _repo.Received(1).AddAsync(contest);
    }

    [Fact]
    public async Task GetUpcomingContestsAsync_ReturnsOnlyUpcomingContests()
    {
        var upcoming = new List<Contest>
        {
            new() { Id = 1, Name = "Round 1", StartTime = DateTime.UtcNow.AddHours(2), EndTime = DateTime.UtcNow.AddHours(4) }
        };
        _repo.GetUpcomingAsync().Returns(upcoming);

        var result = await _sut.GetUpcomingContestsAsync();

        Assert.Single(result);
    }

    [Fact]
    public async Task GetContestsByPlatformAsync_DelegatesToRepository()
    {
        _repo.GetByPlatformAsync("Codeforces").Returns(new List<Contest>());

        await _sut.GetContestsByPlatformAsync("Codeforces");

        await _repo.Received(1).GetByPlatformAsync("Codeforces");
    }

    [Fact]
    public async Task GetPagedContestsAsync_PassesSearchTermToRepository()
    {
        _repo.GetPagedAsync(null, "codeforces", 1, 20)
            .Returns((new List<Contest>(), 0));

        await _sut.GetPagedContestsAsync(null, "codeforces", 1, 20);

        await _repo.Received(1).GetPagedAsync(null, "codeforces", 1, 20);
    }

    [Fact]
    public async Task GetStatsAsync_DelegatesToRepository()
    {
        _repo.GetStatsAsync().Returns(new ContestStats());

        await _sut.GetStatsAsync();

        await _repo.Received(1).GetStatsAsync();
    }
}
