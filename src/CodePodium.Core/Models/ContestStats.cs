namespace CodePodium.Core.Models;

public class ContestStats
{
    public int Total { get; init; }
    public int Upcoming { get; init; }
    public int Ongoing { get; init; }
    public int Finished { get; init; }
    public Dictionary<string, int> ByPlatform { get; init; } = new();
}
