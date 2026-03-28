using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using SimpleRestApi.Model;

namespace SimpleRestApi.Tests;

public class EndpointTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient(new WebApplicationFactoryClientOptions
    {
        // Use HTTPS base address so UseHttpsRedirection doesn't return redirects in tests.
        BaseAddress = new Uri("https://localhost")
    });

    [Fact]
    public async Task GetRoot_ReturnsStatusAndLinks()
    {
        var response = await _client.GetAsync("/");

        response.EnsureSuccessStatusCode();

        using var payload = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var root = payload.RootElement;

        Assert.Equal("Simple REST API is running", root.GetProperty("message").GetString());
        Assert.Equal("/docs", root.GetProperty("docs").GetString());
    }

    [Fact]
    public async Task GetDocs_ReturnsHtml()
    {
        var response = await _client.GetAsync("/docs");

        response.EnsureSuccessStatusCode();
        Assert.Equal("text/html", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task GetWorkouts_ReturnsSeededWorkouts()
    {
        var response = await _client.GetAsync("/workouts");

        response.EnsureSuccessStatusCode();

        var workouts = await response.Content.ReadFromJsonAsync<List<Workout>>();

        Assert.NotNull(workouts);
        Assert.True(workouts.Count >= 2);
    }

    [Fact]
    public async Task PostWorkouts_WithNewWorkout_ReturnsCreated()
    {
        var workout = new Workout(0, "Swim", 1.5, 45, DateTime.Today);

        var response = await _client.PostAsJsonAsync("/workouts", workout);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var createdWorkout = await response.Content.ReadFromJsonAsync<Workout>();
        Assert.NotNull(createdWorkout);
        Assert.True(createdWorkout.Id > 0);
        Assert.Equal("Swim", createdWorkout.Type);
    }

    [Fact]
    public async Task PostWorkouts_WithDuplicateWorkout_ReturnsConflict()
    {
        var duplicate = new Workout(0, "Run", 5.2, 30, DateTime.Now.AddDays(-1));

        var response = await _client.PostAsJsonAsync("/workouts", duplicate);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);

        using var payload = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.Equal("Workout already exists", payload.RootElement.GetProperty("message").GetString());
    }
}
