namespace LogTestAPI;

public static class BulkEndpoints
{
    internal static ILogger _logger { get; set; }
    private const string BaseRoute = "api/test";

    public static void AddEndpoints(this WebApplication app, ILogger logger)
    {
        _logger = logger;
        app.MapPost($"{BaseRoute}/checkHealthy", () =>
        {
            var test = "testing";
            _logger.LogInformation("Checking Healthy : {test}", test.ToString());
            Results.Ok();
        });
    }
}