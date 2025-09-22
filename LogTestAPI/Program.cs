using Serilog;
using Serilog.Events;
using Serilog.Sinks.OpenTelemetry;

var builder = WebApplication.CreateBuilder(args);

using var logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .Filter.ByExcluding(logEvent => logEvent.Properties.TryGetValue("SourceContext", out var sourceContext)
                                    && (sourceContext.ToString().Contains("Microsoft.AspNetCore")
                                        || sourceContext.ToString().Contains("System.Net.Http")))
    .WriteTo.OpenTelemetry(endpoint: "http://otel-serv-discovery-test.namespace-test:4317", protocol : OtlpProtocol.Grpc,
        resourceAttributes: new Dictionary<string, object>
        {
            ["service.name"] = "TestAPI " + Environment.GetEnvironmentVariable("ENV") + "-" + DateTime.UtcNow.ToString("yyyy")
        })
    .WriteTo.Console()
    .CreateLogger();

builder.Logging.AddSerilog(logger);
for (int i = 0; i < 100_000; i++)
{
    logger.Warning("Starting up");
}




var app = builder.Build();



app.MapGet("/", () =>
{
    Results.Ok("Healthy");
});

app.Run();
