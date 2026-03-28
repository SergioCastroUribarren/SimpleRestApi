namespace SimpleRestApi.Model;

public record Workout(
    int Id,
    string Type,
    double Distance,
    int DurationMinutes,
    DateTime Date
);