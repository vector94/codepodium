using System.Text.Json.Serialization;

namespace CodePodium.Infrastructure.ExternalApi.Clist;

public class ClistResponse
{
    [JsonPropertyName("objects")]
    public List<ClistContest> Objects { get; set; } = [];
}

public class ClistContest
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("event")]
    public string Event { get; set; } = string.Empty;

    [JsonPropertyName("resource")]
    public string Resource { get; set; } = string.Empty;

    [JsonPropertyName("start")]
    public DateTime Start { get; set; }

    [JsonPropertyName("end")]
    public DateTime End { get; set; }

    [JsonPropertyName("href")]
    public string Href { get; set; } = string.Empty;
}
