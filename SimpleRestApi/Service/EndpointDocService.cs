namespace SimpleRestApi.Services;

public interface IEndpointDocService
{
    IEnumerable<EndpointDoc> GetEndpointDocs();
}


public class EndpointDocService : IEndpointDocService
{
    private const string workouts = "/workouts";

    public IEnumerable<EndpointDoc> GetEndpointDocs() =>
        [
        new("GET", "/", "API status and quick links.", "/"),
        new("GET", "/docs", "Returns an HTML page that explains all public endpoints.", "/docs"),
        new("GET", "/swagger", "Interactive Swagger UI documentation (Development only).", "/swagger"),
        new("GET", "/openapi/v1.json", "OpenAPI specification document in JSON format.", "/openapi/v1.json"),
        new("GET", workouts, "Returns a list of workouts.", workouts),
        new("POST", workouts, "Creates a new workout. Expects a JSON body with workout details.", workouts),
        new("GET", workouts + "/{id}", "Returns a specific workout by ID.", workouts + "/1"),
        new("GET", workouts + "?type={type}", "Returns workouts filtered by type.", workouts + "?type=Run"),
        new("GET", workouts + "?dateFrom={dateFrom}&dateTo={dateTo}&minDuration={minDuration}", "Returns workouts filtered by date range and minimum duration.", workouts + "?dateFrom=2024-01-01&dateTo=2024-12-31&minDuration=30")
        ];
}


public sealed record EndpointDoc(string Method, string Route, string Description, string ExampleUrl);