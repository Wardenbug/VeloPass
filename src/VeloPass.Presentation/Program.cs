using System.Text.Json;
using System.Text.Json.Serialization;
using Hangfire;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Scalar.AspNetCore;
using VeloPass.Application;
using VeloPass.Infrastructure;
using VeloPass.Infrastructure.Outbox;
using VeloPass.Presentation.Authentication;
using VeloPass.Presentation.Invites;
using VeloPass.Presentation.Organizations;
using VeloPass.Presentation.Users;
using VeloPass.Presentation.SchemeTransformers;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddInfrastructureLayer(builder.Configuration);
builder.Services.AddApplicationLayer(builder.Host);
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
});


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

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(
        new JsonStringEnumConverter(JsonNamingPolicy.CamelCase, allowIntegerValues: false));
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

var recurringJobs = app.Services.GetRequiredService<IRecurringJobManager>();
recurringJobs.AddOrUpdate<IOutboxProcessor>(
    "outbox-processor",
    processor => processor.ProcessAsync(),
    "0/5 * * * * *");

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();

    app.UseHangfireDashboard(options: new DashboardOptions
    {
        Authorization = []
    });
}
app.UseRouting();
app.UseCors("FrontendAppPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.UseHttpsRedirection();

app.MapAuthenticationEndpoints();
app.MapOrganizationEndpoints();
app.MapUsersEndpoints();
app.MapInvitesEndpoints();

await app.RunAsync();