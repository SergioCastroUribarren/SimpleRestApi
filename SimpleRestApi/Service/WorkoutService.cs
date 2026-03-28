namespace SimpleRestApi.WorkoutService;

using SimpleRestApi.Model;

public class WorkoutFilter
{
    public string? Type { get; init; }
}

public interface IWorkoutService
{
    IEnumerable<Workout> GetWorkouts();
    Result<Workout> CreateWorkoutResult(Workout workout);
    Result<Workout> GetWorkout(int id);
    IEnumerable<Workout> GetWorkouts(WorkoutFilter filter);
}


public class WorkoutService : IWorkoutService
{
    private readonly List<Workout> _workouts = new()
    {
        new Workout(1, "Run", 5.2, 30, DateTime.Now.AddDays(-1)),
        new Workout(2, "Bike", 20, 60, DateTime.Now.AddDays(-2))
    };

    public Result<Workout> CreateWorkoutResult(Workout workout)
    {
        const double tolerance = 0.01;
        var exists = _workouts.Any(w =>
                    w.Type == workout.Type &&
                    w.Date.Date == workout.Date.Date &&
                    w.DurationMinutes == workout.DurationMinutes &&
                    Math.Abs(w.Distance - workout.Distance) < tolerance
                    );

        if (exists)
        {
            return Result<Workout>.Failure("Workout already exists");
        }

        var newId = _workouts.Any() ? _workouts.Max(w => w.Id) + 1 : 1;

        var newWorkout = workout with { Id = newId };

        _workouts.Add(newWorkout);

        return Result<Workout>.Success(newWorkout);
    }

    public Result<Workout> GetWorkout(int id)
    {
        var workout = _workouts.FirstOrDefault(w => w.Id == id);
        if (workout == null)
        {
            return Result<Workout>.Failure("Workout not found");
        }
        return Result<Workout>.Success(workout);
    }

    public IEnumerable<Workout> GetWorkouts()
    {
        return _workouts;
    }

    public IEnumerable<Workout> GetWorkouts(WorkoutFilter filter)
    {
        var query = _workouts.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(filter.Type))
        {
            query = query.Where(w => w.Type.Equals(filter.Type, StringComparison.OrdinalIgnoreCase));
        }

        return query;
    }
}