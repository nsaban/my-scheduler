using Microsoft.EntityFrameworkCore;
using MyScheduler.Api.Endpoints;
using MyScheduler.Api.Infrastructure;
using MyScheduler.Application.DependencyInjection;
using MyScheduler.Infrastructure.DependencyInjection;
using MyScheduler.Persistence;
using MyScheduler.Persistence.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddInfrastructure();

builder.Services.AddHealthChecks();
builder.Services.AddOpenApi();

builder.Services.AddExceptionHandler<ValidationExceptionHandler>();
builder.Services.AddExceptionHandler<NotFoundExceptionHandler>();
builder.Services.AddExceptionHandler<DomainExceptionHandler>();
builder.Services.AddExceptionHandler<ConcurrencyExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

app.UseExceptionHandler();

app.MapHealthChecks("/health");
app.MapOpenApi();
app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "MyScheduler API v1"));

app.MapEventEndpoints();
app.MapAttendeeEndpoints();

if (app.Configuration.GetValue<bool>("ApplyMigrationsOnStartup"))
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();
}

app.Run();

public partial class Program
{
}
