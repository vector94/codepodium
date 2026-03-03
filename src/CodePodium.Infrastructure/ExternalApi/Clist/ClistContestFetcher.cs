using System.Text.Json;
using CodePodium.Core.Interfaces;
using CodePodium.Core.Models;

namespace CodePodium.Infrastructure.ExternalApi.Clist;

public class ClistContestFetcher(HttpClient httpClient, string apiKey, string username) : IContestFetcher
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    public string Platform => "Clist";

    public async Task<IEnumerable<Contest>> FetchContestsAsync()
    {
        var url = $"https://clist.by/api/v4/contest/?username={username}&api_key={apiKey}&upcoming=true&order_by=start&limit=50";
        var response = await httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        var data = JsonSerializer.Deserialize<ClistResponse>(json, JsonOptions);

        if (data?.Objects is null)
            return [];

        return data.Objects.Select(c => new Contest
        {
            ExternalId = c.Id.ToString(),
            Name = c.Event,
            Platform = c.Resource,
            StartTime = DateTime.SpecifyKind(c.Start, DateTimeKind.Utc),
            EndTime = DateTime.SpecifyKind(c.End, DateTimeKind.Utc),
            Url = c.Href,
            Status = c.Start > DateTime.UtcNow ? ContestStatus.Upcoming : ContestStatus.Ongoing
        });
    }
}
