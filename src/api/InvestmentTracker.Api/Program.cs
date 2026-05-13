using InvestmentTracker.Infrastructure;
using InvestmentTracker.Api.Contracts.Auth;
using InvestmentTracker.Api.Middleware;
using FluentValidation;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddDataProtection();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<ValidationExceptionHandler>();
builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddValidatorsFromAssemblyContaining<RegisterUserRequestValidator>();
builder.Services.AddInfrastructure(
    builder.Configuration.GetConnectionString("InvestmentTrackerDb")
        ?? throw new InvalidOperationException("Connection string 'InvestmentTrackerDb' is not configured."));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseExceptionHandler();
app.UseHttpsRedirection();

app.MapControllers();

app.Run();
