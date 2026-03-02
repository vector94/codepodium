namespace CodePodium.Core.Models;

public class Contest
{
    public int Id { get; set; }
    public string ExternalId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Platform { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string Url { get; set; } = string.Empty;
    public ContestStatus Status { get; set; }
    public DateTime FetchedAt { get; set; } = DateTime.UtcNow;

    public TimeSpan Duration => EndTime - StartTime;
    public bool IsUpcoming => StartTime > DateTime.UtcNow;
    public bool IsOngoing => StartTime <= DateTime.UtcNow && EndTime > DateTime.UtcNow;
}

public enum ContestStatus
{
    Upcoming,
    Ongoing,
    Finished
}
