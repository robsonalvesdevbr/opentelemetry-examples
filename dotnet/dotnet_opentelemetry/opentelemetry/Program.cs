using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var serviceName = Environment.GetEnvironmentVariable("INSTANCE_NAME") ?? "dotnet-opentelemetry";
var serviceVersion = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "undefined";
var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

var resourceAttributes = new Dictionary<string, object>
{
    ["runtime.os.deescription"] = RuntimeInformation.OSDescription,
    ["runtime.framework.description"] = RuntimeInformation.FrameworkDescription,
    ["deployment.environment"] = environment,
    ["service.name"] = serviceName,
    ["service.namespace"] = Assembly.GetExecutingAssembly().GetName().Name ?? "undefined",
    ["service.version"] = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "undefined"
};

var MyActivitySource = new ActivitySource(serviceName);
using var activity = MyActivitySource.StartActivity("SayHello");
activity?.SetTag("foo", 1);
activity?.SetTag("bar", "Hello, World!");
activity?.SetTag("baz", new int[] { 1, 2, 3 });

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
            // // Metrics provided by System.Net libraries
            // .AddMeter("System.Net.Http")
            // .AddMeter("System.Net.NameResolution")
            // // Metrics provided by ASP.NET libraries
            // .AddMeter("Microsoft.AspNetCore.Hosting")
            // .AddMeter("Microsoft.AspNetCore.Server.Kestrel")
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
