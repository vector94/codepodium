using System.Text;
using System.Text.Json;
using CodePodium.Core.Interfaces;
using CodePodium.Core.Models;

namespace CodePodium.Infrastructure.ExternalApi.LeetCode;

public class LeetCodeContestFetcher(HttpClient httpClient) : IContestFetcher
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    public string Platform => "LeetCode";

    public async Task<IEnumerable<Contest>> FetchContestsAsync()
    {
        var query = """
            {"query":"{ allContests { title titleSlug startTime duration } }"}
            """;

        var request = new HttpRequestMessage(HttpMethod.Post, "https://leetcode.com/graphql")
        {
            Content = new StringContent(query, Encoding.UTF8, "application/json")
        };
        request.Headers.Add("Referer", "https://leetcode.com");

        var response = await httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);

        if (!doc.RootElement.TryGetProperty("data", out var data) ||
            !data.TryGetProperty("allContests", out var contests))
            return [];

        var results = new List<Contest>();
        foreach (var c in contests.EnumerateArray().Take(50))
        {
            var title = c.GetProperty("title").GetString() ?? string.Empty;
            var slug = c.GetProperty("titleSlug").GetString() ?? string.Empty;
            var startSeconds = c.GetProperty("startTime").GetInt64();
            var durationSeconds = c.GetProperty("duration").GetInt64();

            var start = DateTimeOffset.FromUnixTimeSeconds(startSeconds).UtcDateTime;
            var end = start.AddSeconds(durationSeconds);
            var now = DateTime.UtcNow;

            results.Add(new Contest
            {
                ExternalId = slug,
                Name = title,
                Platform = Platform,
                StartTime = start,
                EndTime = end,
                Url = $"https://leetcode.com/contest/{slug}",
                Status = now < start ? ContestStatus.Upcoming
                       : now < end ? ContestStatus.Ongoing
                       : ContestStatus.Finished
            });
        }

        return results;
    }
}
