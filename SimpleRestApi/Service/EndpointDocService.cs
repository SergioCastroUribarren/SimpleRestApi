namespace SimpleRestApi.EndpointDocService;

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
        new("POST", workouts, "Creates a new workout. Expects a JSON body with workout details.", workouts)
        ];
}


public sealed record EndpointDoc(string Method, string Route, string Description, string ExampleUrl);