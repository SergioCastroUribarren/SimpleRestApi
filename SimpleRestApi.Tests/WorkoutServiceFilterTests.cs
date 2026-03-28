using SimpleRestApi.Services;

namespace SimpleRestApi.Tests;

/// <summary>
/// Direct unit tests for WorkoutService filter logic (DateFrom, DateTo, MinDuration, MaxDuration).
/// These tests bypass the HTTP layer and call the service directly.
/// </summary>
public class WorkoutServiceFilterTests
{
    // Each test gets a fresh service instance with seeded data:
    //   Workout(1, "Run",  distance=5.2,  duration=30 min, date=today-1)
    //   Workout(2, "Bike", distance=20.0, duration=60 min, date=today-2)
    private static WorkoutService CreateService() => new();
    // ── DateFrom ─────────────────────────────────────────────────────────────

    [Fact]
    public void GetWorkouts_DateFrom_Yesterday_ReturnsBothWorkouts()
    {
        var service = CreateService();
        var filter = new WorkoutFilter { DateFrom = DateTime.Today.AddDays(-2) };

        var result = service.GetWorkouts(filter).ToList();

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public void GetWorkouts_DateFrom_Today_ReturnsNoWorkouts()
    {
        var service = CreateService();
        var filter = new WorkoutFilter { DateFrom = DateTime.Today };

        var result = service.GetWorkouts(filter).ToList();

        Assert.Empty(result);
    }

    [Fact]
    public void GetWorkouts_DateFrom_TwoDaysAgo_ExcludesOlderWorkout()
    {
        var service = CreateService();
        // From exactly yesterday — should include Run (day-1) but not Bike (day-2)
        var filter = new WorkoutFilter { DateFrom = DateTime.Today.AddDays(-1) };

        var result = service.GetWorkouts(filter).ToList();

        Assert.Single(result);
        Assert.Equal("Run", result[0].Type);
    }

    // ── DateTo ───────────────────────────────────────────────────────────────

    [Fact]
    public void GetWorkouts_DateTo_Yesterday_ReturnsBothWorkouts()
    {
        var service = CreateService();
        var filter = new WorkoutFilter { DateTo = DateTime.Today.AddDays(-1) };

        var result = service.GetWorkouts(filter).ToList();

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public void GetWorkouts_DateTo_TwoDaysAgo_ReturnsOnlyBike()
    {
        var service = CreateService();
        // Up to two days ago — only Bike (day-2) qualifies
        var filter = new WorkoutFilter { DateTo = DateTime.Today.AddDays(-2) };

        var result = service.GetWorkouts(filter).ToList();

        Assert.Single(result);
        Assert.Equal("Bike", result[0].Type);
    }

    [Fact]
    public void GetWorkouts_DateTo_ThreeDaysAgo_ReturnsEmpty()
    {
        var service = CreateService();
        var filter = new WorkoutFilter { DateTo = DateTime.Today.AddDays(-3) };

        var result = service.GetWorkouts(filter).ToList();

        Assert.Empty(result);
    }

    // ── DateFrom + DateTo range ───────────────────────────────────────────────

    [Fact]
    public void GetWorkouts_DateRange_ExactMatch_ReturnsSingleWorkout()
    {
        var service = CreateService();
        var filter = new WorkoutFilter
        {
            DateFrom = DateTime.Today.AddDays(-1),
            DateTo = DateTime.Today.AddDays(-1)
        };

        var result = service.GetWorkouts(filter).ToList();

        Assert.Single(result);
        Assert.Equal("Run", result[0].Type);
    }

    // ── MinDuration ──────────────────────────────────────────────────────────

    [Fact]
    public void GetWorkouts_MinDuration_45_ReturnsOnlyBike()
    {
        var service = CreateService();
        // Run=30 min, Bike=60 min — only Bike meets >= 45
        var filter = new WorkoutFilter { MinDuration = 45 };

        var result = service.GetWorkouts(filter).ToList();

        Assert.Single(result);
        Assert.Equal("Bike", result[0].Type);
    }

    [Fact]
    public void GetWorkouts_MinDuration_30_ReturnsBothWorkouts()
    {
        var service = CreateService();
        var filter = new WorkoutFilter { MinDuration = 30 };

        var result = service.GetWorkouts(filter).ToList();

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public void GetWorkouts_MinDuration_61_ReturnsEmpty()
    {
        var service = CreateService();
        var filter = new WorkoutFilter { MinDuration = 61 };

        var result = service.GetWorkouts(filter).ToList();

        Assert.Empty(result);
    }

    // ── MaxDuration ──────────────────────────────────────────────────────────

    [Fact]
    public void GetWorkouts_MaxDuration_45_ReturnsOnlyRun()
    {
        var service = CreateService();
        // Run=30 min, Bike=60 min — only Run meets <= 45
        var filter = new WorkoutFilter { MaxDuration = 45 };

        var result = service.GetWorkouts(filter).ToList();

        Assert.Single(result);
        Assert.Equal("Run", result[0].Type);
    }

    [Fact]
    public void GetWorkouts_MaxDuration_60_ReturnsBothWorkouts()
    {
        var service = CreateService();
        var filter = new WorkoutFilter { MaxDuration = 60 };

        var result = service.GetWorkouts(filter).ToList();

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public void GetWorkouts_MaxDuration_29_ReturnsEmpty()
    {
        var service = CreateService();
        var filter = new WorkoutFilter { MaxDuration = 29 };

        var result = service.GetWorkouts(filter).ToList();

        Assert.Empty(result);
    }

    // ── MinDuration + MaxDuration range ──────────────────────────────────────

    [Fact]
    public void GetWorkouts_DurationRange_30To60_ReturnsBothWorkouts()
    {
        var service = CreateService();
        var filter = new WorkoutFilter { MinDuration = 30, MaxDuration = 60 };

        var result = service.GetWorkouts(filter).ToList();

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public void GetWorkouts_DurationRange_40To50_ReturnsEmpty()
    {
        var service = CreateService();
        var filter = new WorkoutFilter { MinDuration = 40, MaxDuration = 50 };

        var result = service.GetWorkouts(filter).ToList();

        Assert.Empty(result);
    }

    // ── Combined filters ─────────────────────────────────────────────────────

    [Fact]
    public void GetWorkouts_TypeAndMinDuration_ReturnsMatchingWorkout()
    {
        var service = CreateService();
        var filter = new WorkoutFilter { Type = "Bike", MinDuration = 45 };

        var result = service.GetWorkouts(filter).ToList();

        Assert.Single(result);
        Assert.Equal("Bike", result[0].Type);
        Assert.Equal(60, result[0].DurationMinutes);
    }

    [Fact]
    public void GetWorkouts_TypeAndMinDuration_NoMatch_ReturnsEmpty()
    {
        var service = CreateService();
        // Run exists but its 30 min doesn't meet MinDuration=45
        var filter = new WorkoutFilter { Type = "Run", MinDuration = 45 };

        var result = service.GetWorkouts(filter).ToList();

        Assert.Empty(result);
    }
}
