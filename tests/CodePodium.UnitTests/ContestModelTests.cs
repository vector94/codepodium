using CodePodium.Core.Models;

namespace CodePodium.UnitTests;

public class ContestModelTests
{
    [Fact]
    public void IsUpcoming_ReturnsTrue_WhenStartTimeIsInFuture()
    {
        var contest = new Contest { StartTime = DateTime.UtcNow.AddHours(1), EndTime = DateTime.UtcNow.AddHours(3) };
        Assert.True(contest.IsUpcoming);
    }

    [Fact]
    public void IsUpcoming_ReturnsFalse_WhenStartTimeIsInPast()
    {
        var contest = new Contest { StartTime = DateTime.UtcNow.AddHours(-2), EndTime = DateTime.UtcNow.AddHours(1) };
        Assert.False(contest.IsUpcoming);
    }

    [Fact]
    public void IsOngoing_ReturnsTrue_WhenNowIsBetweenStartAndEnd()
    {
        var contest = new Contest { StartTime = DateTime.UtcNow.AddHours(-1), EndTime = DateTime.UtcNow.AddHours(1) };
        Assert.True(contest.IsOngoing);
    }

    [Fact]
    public void IsOngoing_ReturnsFalse_WhenContestHasEnded()
    {
        var contest = new Contest { StartTime = DateTime.UtcNow.AddHours(-3), EndTime = DateTime.UtcNow.AddHours(-1) };
        Assert.False(contest.IsOngoing);
    }

    [Fact]
    public void Duration_ReturnsCorrectTimeSpan()
    {
        var start = DateTime.UtcNow;
        var end = start.AddHours(2.5);
        var contest = new Contest { StartTime = start, EndTime = end };

        Assert.Equal(TimeSpan.FromHours(2.5), contest.Duration);
    }
}
