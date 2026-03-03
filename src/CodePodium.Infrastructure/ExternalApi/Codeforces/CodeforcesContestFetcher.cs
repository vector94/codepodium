using System.Text.Json;
using CodePodium.Core.Interfaces;
using CodePodium.Core.Models;

namespace CodePodium.Infrastructure.ExternalApi.Codeforces;

public class CodeforcesContestFetcher(HttpClient httpClient) : IContestFetcher
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    public string Platform => "Codeforces";

    public async Task<IEnumerable<Contest>> FetchContestsAsync()
    {
        var response = await httpClient.GetAsync("https://codeforces.com/api/contest.list");
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        var data = JsonSerializer.Deserialize<CodeforcesResponse>(json, JsonOptions);

        if (data?.Status != "OK" || data.Result is null)
            return [];

        return data.Result
            .Where(c => c.Phase is "BEFORE" or "CODING" or "FINISHED")
            .Take(100)
            .Select(c =>
            {
                var start = DateTimeOffset.FromUnixTimeSeconds(c.StartTimeSeconds).UtcDateTime;
                var end = start.AddSeconds(c.DurationSeconds);
                var status = c.Phase switch
                {
                    "CODING" => ContestStatus.Ongoing,
                    "FINISHED" => ContestStatus.Finished,
                    _ => ContestStatus.Upcoming
                };
                return new Contest
                {
                    ExternalId = c.Id.ToString(),
                    Name = c.Name,
                    Platform = Platform,
                    StartTime = start,
                    EndTime = end,
                    Url = $"https://codeforces.com/contest/{c.Id}",
                    Status = status
                };
            });
    }
}
