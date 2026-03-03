using System.Text.Json.Serialization;

namespace CodePodium.Infrastructure.ExternalApi.Codeforces;

public class CodeforcesResponse
{
    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("result")]
    public List<CodeforcesContest> Result { get; set; } = [];
}

public class CodeforcesContest
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("phase")]
    public string Phase { get; set; } = string.Empty;

    [JsonPropertyName("startTimeSeconds")]
    public long StartTimeSeconds { get; set; }

    [JsonPropertyName("durationSeconds")]
    public long DurationSeconds { get; set; }
}
