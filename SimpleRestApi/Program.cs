using SimpleRestApi.EndpointDocService;
using SimpleRestApi.WorkoutService;
using SimpleRestApi.Model;


const string workoutsConstant = "/workouts";

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

//Register the service that provides workout data. The IWorkoutService interface and WorkoutService implementation are defined in separate files for clarity and testability, but this is where the service is added to the app's dependency injection container.
builder.Services.AddSingleton<IWorkoutService, WorkoutService>();
builder.Services.AddSingleton<IEndpointDocService, EndpointDocService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    //Enable Swagger middleware
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Browser Root page. Open in browser: http://localhost/
app.MapGet("/", () =>
{
    return Results.Ok(new
    {
        message = "Simple REST API is running",
        docs = "/docs",
        endpoints = new[]
        {
            "/"         ,
            "/docs",
            "/swagger",
            "/openapi/v1.json",
            "/workouts"
        }
    });
});


// Browser documentation page. Open in browser: http://localhost/docs
app.MapGet("/docs", (HttpRequest request, IEndpointDocService service) =>
{
    var baseUrl = $"{request.Scheme}://{request.Host}";
    var items = string.Join("", service.GetEndpointDocs().Select(doc =>
        $"<li><strong>{doc.Method}</strong> <code>{doc.Route}</code><br/>{doc.Description}<br/>Example: <a href=\"{baseUrl}{doc.ExampleUrl}\">{baseUrl}{doc.ExampleUrl}</a></li>"));

    var html = $"""
<!doctype html>
<html lang="en">
<head>
  <meta charset="utf-8" />
  <meta name="viewport" content="width=device-width, initial-scale=1" />
  <title>Activity Tracker API Docs</title>
</head>
<body>
  <h1>Activity Tracker API</h1>
  <p>Available public endpoints and how to call them.</p>  
  <ul>
    {items}
  </ul>
</body>
</html>
""";

    return Results.Content(html, "text/html");
});


// API endpoint to get workouts with optional type filter. Open in browser: http://localhost/workouts or http://localhost/workouts?type=Run
app.MapGet(workoutsConstant, ([AsParameters] WorkoutFilter filter, IWorkoutService service) =>
{
    var workouts = service.GetWorkouts(filter);
    return Results.Ok(workouts);
});

// API endpoint to create a new workout. Use a tool like Postman or curl to send a POST request with a JSON body to http://localhost/workouts
app.MapPost(workoutsConstant, (Workout workout, IWorkoutService service) =>
{
    var result = service.CreateWorkoutResult(workout);

    if (result.IsSuccess)
    {
        return Results.Created($"/workouts/{result.Value!.Id}", result.Value!);
    }

    return Results.Conflict(new { message = result.Error });
});

// API endpoint to get a workout by ID. Open in browser: http://localhost/workouts/1
app.MapGet($"{workoutsConstant}/{{id:int}}", (int id, IWorkoutService service) =>
{
    var result = service.GetWorkout(id);

    if (result.IsSuccess)
    {
        return Results.Ok(result.Value);
    }

    return Results.NotFound(new { message = result.Error });
});


app.Run();

public partial class Program;
