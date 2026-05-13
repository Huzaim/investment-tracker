using InvestmentTracker.Application.Assets;
using InvestmentTracker.Application.Auth;
using InvestmentTracker.Application.Common.Behaviors;
using InvestmentTracker.Infrastructure.Auth;
using InvestmentTracker.Infrastructure.Assets;
using InvestmentTracker.Infrastructure.Persistence;
using InvestmentTracker.Infrastructure.Persistence.Users;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace InvestmentTracker.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        string connectionString)
    {
        services.AddDbContext<InvestmentTrackerDbContext>(options =>
            options.UseNpgsql(connectionString));
        services
            .AddIdentityCore<ApplicationUser>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 8;
                options.Password.RequiredUniqueChars = 1;
            })
            .AddRoles<IdentityRole<Guid>>()
            .AddEntityFrameworkStores<InvestmentTrackerDbContext>()
            .AddDefaultTokenProviders();
        services.AddValidatorsFromAssemblyContaining<RegisterUserCommandValidator>();
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddScoped<IUserRegistrationPort, IdentityUserRegistrationPort>();
        services.AddScoped<IAssetQueryService, AssetQueryService>();
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(InvestmentTracker.Application.Auth.RegisterUserCommand).Assembly));

        return services;
    }
}
