using System.Reflection;
using System.Runtime.InteropServices;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var serviceName = "RobsonAlves.MyApp";
var serviceVersion = "1.0.0";
var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

var resourceAttributes = new Dictionary<string, object>
{
    ["runtime.os.deescription"] = RuntimeInformation.OSDescription,
    ["runtime.framework.description"] = RuntimeInformation.FrameworkDescription,
    ["deployment.environment"] = environment,
    ["service.name"] = "RobsonAlves.Backend.Microservice.Client",
    ["service.namespace"] = Assembly.GetExecutingAssembly().GetName().Name ?? "undefined",
    ["service.version"] = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "undefined"
};


var builder = WebApplication.CreateBuilder(args);

//var serviceName = typeof(Program).Namespace ?? "Unknown";
//var serviceVersion = typeof(Program).Assembly.GetName().Version?.ToString() ?? "Unknown";

// Add services to the container.
builder.Services.AddOpenTelemetry()
    .ConfigureResource(res => {
        res.AddService(serviceName, serviceVersion);
        res.AddAttributes(resourceAttributes);
    })
    .WithTracing(tracing => tracing
        // The rest of your setup code goes here
        .AddOtlpExporter(options =>
        {
            options.Endpoint = new Uri("http://otel-collector:4317");
            options.Protocol = OtlpExportProtocol.Grpc;
        })
        .AddConsoleExporter())
    .WithMetrics(metrics => metrics
        // The rest of your setup code goes here
        .AddOtlpExporter(options =>
        {
            options.Endpoint = new Uri("http://otel-collector:4317");
            options.Protocol = OtlpExportProtocol.Grpc;
        }));
    //.UseOtlpExporter(OtlpExportProtocol.Grpc, new Uri("http://localhost:4317"));

builder.Logging.AddOpenTelemetry(logging => {

    logging.SetResourceBuilder(ResourceBuilder.CreateDefault()
        .AddService(serviceName, serviceVersion)
        .AddAttributes(resourceAttributes))
    .AddConsoleExporter();
    logging.IncludeFormattedMessage = true;

    // The rest of your setup code goes here
    logging.AddOtlpExporter(options =>
    {
        options.Endpoint = new Uri("http://otel-collector:4317");
        options.Protocol = OtlpExportProtocol.Grpc;
    });
});




builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
