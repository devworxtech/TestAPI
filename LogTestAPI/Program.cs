using Serilog;
using Serilog.Events;
using Serilog.Sinks.OpenTelemetry;

var builder = WebApplication.CreateBuilder(args);


var app = builder.Build();

using var logger = new LoggerConfiguration()
    .MinimumLevel.Is(LogEventLevel.Information)
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .Filter.ByExcluding(logEvent => logEvent.Properties.TryGetValue("SourceContext", out var sourceContext)
                                    && (sourceContext.ToString().Contains("Microsoft.AspNetCore")
                                        || sourceContext.ToString().Contains("System.Net.Http")))
    .WriteTo.OpenTelemetry(endpoint: "http://opentelemetry-serv-discovery-test.namespace-test:4317", protocol : OtlpProtocol.Grpc,
        resourceAttributes: new Dictionary<string, object>
        {
            ["service.name"] = "TestAPI " + Environment.GetEnvironmentVariable("ENV") + "-" + DateTime.UtcNow.ToString("yyyy")
        })
    .CreateLogger();

builder.Logging.AddSerilog(logger);
logger.Information("Starting up");

app.MapGet("/", () =>
{
    Results.Ok("Healthy");
});

app.Run();
