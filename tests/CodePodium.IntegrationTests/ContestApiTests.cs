using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using CodePodium.Core.Models;
using CodePodium.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;

namespace CodePodium.IntegrationTests;

public class ContestApiTests(TestWebAppFactory factory) : IClassFixture<TestWebAppFactory>
{
    private readonly HttpClient _client = factory.CreateClient();
    private static readonly JsonSerializerOptions JsonOpts = new(JsonSerializerDefaults.Web);

    private async Task<Contest> SeedContestAsync(string externalId = "seed-1", string platform = "Codeforces", string name = "Test Contest")
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var contest = new Contest
        {
            ExternalId = externalId,
            Name = name,
            Platform = platform,
            StartTime = DateTime.UtcNow.AddHours(1),
            EndTime = DateTime.UtcNow.AddHours(3),
            Url = "https://codeforces.com",
            Status = ContestStatus.Upcoming
        };
        db.Contests.Add(contest);
        await db.SaveChangesAsync();
        return contest;
    }

    [Fact]
    public async Task GetContests_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/contests");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetUpcoming_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/contests/upcoming");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetContestById_ReturnsNotFound_WhenMissing()
    {
        var response = await _client.GetAsync("/api/contests/99999");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetContestById_ReturnsContest_WhenExists()
    {
        var contest = await SeedContestAsync("get-by-id-1");
        var response = await _client.GetAsync($"/api/contests/{contest.Id}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<Contest>(JsonOpts);
        Assert.Equal("Test Contest", result?.Name);
    }

    [Fact]
    public async Task GetContests_FiltersByPlatform()
    {
        await SeedContestAsync("filter-cf-2", "Codeforces");
        await SeedContestAsync("filter-lc-2", "LeetCode");

        var response = await _client.GetAsync("/api/contests?platform=LeetCode");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var json = await response.Content.ReadFromJsonAsync<JsonElement>(JsonOpts);
        var items = json.GetProperty("items").Deserialize<List<Contest>>(JsonOpts);
        Assert.NotNull(items);
        Assert.All(items!, c => Assert.Equal("LeetCode", c.Platform));
    }

    [Fact]
    public async Task GetContests_SearchByName_ReturnsMatchingContests()
    {
        await SeedContestAsync("search-1", "Codeforces", "Codeforces Round 900");
        await SeedContestAsync("search-2", "LeetCode", "Weekly Contest 400");

        var response = await _client.GetAsync("/api/contests?search=Round");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var json = await response.Content.ReadFromJsonAsync<JsonElement>(JsonOpts);
        var items = json.GetProperty("items").Deserialize<List<Contest>>(JsonOpts);
        Assert.NotNull(items);
        Assert.All(items!, c => Assert.Contains("Round", c.Name));
    }
}

public class SyncApiTests(TestWebAppFactory factory) : IClassFixture<TestWebAppFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task PostSync_ReturnsOk()
    {
        var response = await _client.PostAsync("/api/contests/sync", null);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}

public class StatsApiTests(TestWebAppFactory factory) : IClassFixture<TestWebAppFactory>
{
    private readonly HttpClient _client = factory.CreateClient();
    private static readonly JsonSerializerOptions JsonOpts = new(JsonSerializerDefaults.Web);

    [Fact]
    public async Task GetStats_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/contests/stats");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetStats_ReturnsTotalCount()
    {
        var response = await _client.GetAsync("/api/contests/stats");
        var json = await response.Content.ReadFromJsonAsync<JsonElement>(JsonOpts);
        Assert.True(json.TryGetProperty("total", out _));
    }
}
