using HotelLakeview.Application.DependencyInjection;
using HotelLakeview.Infrastructure.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using HotelLakeview.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// Add services to the container.
builder.Services.AddControllers();

// Swagger / OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<HotelLakeviewDbContext>(options =>
    options.UseNpgsql(connectionString));

// Application + Infrastructure
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// Authorization runko valmiiksi myöhempää käyttöä varten
builder.Services.AddAuthorization();

builder.Services
    .AddHealthChecks()
    .AddDbContextCheck<HotelLakeviewDbContext>(
        name: "database",
        failureStatus: HealthStatus.Unhealthy,
        tags: new[] { "db", "postgres", "ready" });

var app = builder.Build();

var enableSwagger = app.Environment.IsDevelopment() ||
                    builder.Configuration.GetValue<bool>("Swagger:Enabled");

if (enableSwagger)
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();

app.MapHealthChecks("/health");

app.UseAuthorization();

app.MapControllers();

//await HotelLakeviewSeedData.SeedAsync(app.Services);

app.Run();