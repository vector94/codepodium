using CodePodium.Core.Models;

namespace CodePodium.Core.Interfaces;

public interface IContestFetcher
{
    string Platform { get; }
    Task<IEnumerable<Contest>> FetchContestsAsync();
}
