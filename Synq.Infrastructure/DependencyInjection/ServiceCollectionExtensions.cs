using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Synq.Application.Common.Interfaces;
using Synq.Infrastructure.Behaviours;
using Synq.Infrastructure.Identity;
using Synq.Infrastructure.Persistence;

namespace Synq.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options => options.UseNpgsql(configuration["ConnectionString"]));
        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<AppDbContext>());
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IChatService, ChatService>();
        services.AddScoped(typeof(IJsonHelper<>) , typeof(JsonHelper<>));
        return services;
    }
}