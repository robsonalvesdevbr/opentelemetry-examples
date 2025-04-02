using System.Reflection;
using System.Runtime.InteropServices;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var serviceName = "dotnet_opentelemetry.opentelemetry_2";
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

builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource
        .AddService(
            serviceName: serviceName,
            serviceVersion: serviceVersion)
        .AddAttributes(resourceAttributes))
    .WithTracing(tracing => tracing
        .AddSource(serviceName)
        .AddHttpClientInstrumentation()
        .AddAspNetCoreInstrumentation()
        .AddConsoleExporter()
        .AddOtlpExporter()
    )
    .WithMetrics(metrics => 
        metrics
            .AddMeter(serviceName)
            .AddHttpClientInstrumentation()
            .AddAspNetCoreInstrumentation()
            .AddConsoleExporter()
            .AddOtlpExporter()
        );

builder.Logging.AddOpenTelemetry(options => {
    options.IncludeScopes = true;
    options.IncludeFormattedMessage = true;
    options.ParseStateValues = true;

    options
    .SetResourceBuilder(ResourceBuilder.CreateDefault()
    .AddService(
        serviceName: serviceName,
        serviceVersion: serviceVersion)
    .AddAttributes(resourceAttributes))
    .AddConsoleExporter()
    .AddOtlpExporter();
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
