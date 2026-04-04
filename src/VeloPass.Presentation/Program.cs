using System.Security.Cryptography;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Scalar.AspNetCore;
using VeloPass.Application;
using VeloPass.Infrastructure;
using VeloPass.Presentation;
using VeloPass.Presentation.Authentication;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddInfrastructureLayer(builder.Configuration);
builder.Services.AddApplicationLayer(builder.Host);
builder.Services.AddOpenApi();

builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendAppPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});


builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource.AddService(builder.Environment.ApplicationName))
    .WithTracing(tracing => tracing
        .AddHttpClientInstrumentation()
        .AddAspNetCoreInstrumentation())
    .WithMetrics(metrics => metrics
        .AddHttpClientInstrumentation()
        .AddAspNetCoreInstrumentation()
        .AddRuntimeInstrumentation())
    .UseOtlpExporter();

builder.Logging.AddOpenTelemetry(options =>
{
    options.IncludeScopes = true;
    options.IncludeFormattedMessage = true;
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}
app.UseRouting();
app.UseCors("FrontendAppPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.UseHttpsRedirection();

app.MapAuthenticationEndpoints();

await app.RunAsync();