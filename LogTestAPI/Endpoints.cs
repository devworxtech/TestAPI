namespace LogTestAPI;

public class BulkEndpoints
{
    private const string BaseRoute = "api/test";

    public static void AddEndpoints(WebApplication app)
    {
        app.MapGet($"{BaseRoute}/checkHealthy", CheckHealthy);
    }
    
    internal static async Task<IResult> CheckHealthy()
    {
        var test = "testing";
        // logger.LogInformation("Checking healthy {test}", test);
        return Results.Ok();
    }
}