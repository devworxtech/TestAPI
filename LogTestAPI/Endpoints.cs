namespace LogTestAPI;

public static class BulkEndpoints
{
    internal static ILogger _logger { get; set; }
    private const string BaseRoute = "api/test";

    public static void AddEndpoints(this WebApplication app, ILogger logger)
    {
        _logger = logger;
        app.MapGet($"{BaseRoute}/checkHealthy", CheckHealthy);
    }
    
    internal static async Task<IResult> CheckHealthy()
    {
        var test = "testing";
        _logger.LogInformation("Checking healthy {test}", test);
        return Results.Ok();
    }
}