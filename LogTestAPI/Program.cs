using LogTestAPI;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.OpenTelemetry;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OpenTelemetry.Metrics;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .Filter.ByExcluding(logEvent => logEvent.Properties.TryGetValue("SourceContext", out var sourceContext)
                                    && (sourceContext.ToString().Contains("Microsoft.AspNetCore")
                                        || sourceContext.ToString().Contains("System.Net.Http")))
    .WriteTo.OpenTelemetry(endpoint: "http://otel-serv-discovery-test.namespace-test:4317", protocol : OtlpProtocol.Grpc,
        resourceAttributes: new Dictionary<string, object>
        {
            ["service.name"] = "TestAPI",
            ["deployment.environment"] = Environment.GetEnvironmentVariable("ENV") ?? "dev",
            ["service.version"] = "1.0.0"
        })
    .WriteTo.Console()
    .CreateLogger();

builder.Logging.AddSerilog(Log.Logger);

// builder.Services.AddOpenTelemetry()
//     .ConfigureResource(r => r.AddService(
//         serviceName: "TestAPI",
//         serviceVersion: "1.0.0"))
//     .WithTracing(tracing =>
//     {
//         tracing
//             .AddAspNetCoreInstrumentation()  // incoming HTTP spans
//             .AddHttpClientInstrumentation()  // outgoing HTTP spans
//             .AddOtlpExporter(o =>
//             {
//                 o.Endpoint = new Uri("http://otel-serv-discovery-test.namespace-test:4317");
//                 o.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.Grpc;
//             });
//     })
//     .WithMetrics(metrics =>
//     {
//         metrics
//             .AddAspNetCoreInstrumentation()  // request duration, count
//             .AddHttpClientInstrumentation()  // outgoing request metrics
//             .AddOtlpExporter(o =>
//             {
//                 o.Endpoint = new Uri("http://otel-serv-discovery-test.namespace-test:4317");
//                 o.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.Grpc;
//             });
//     });
for (int i = 0; i < 1_000; i++)
{
    Log.Logger.Warning("Starting up counter {counter}", i.ToString());
}
//
var app = builder.Build();

app.MapGet("/", () =>
{
    Results.Ok("Healthy");
});
app.AddEndpoints(app.Logger);
app.Run();
