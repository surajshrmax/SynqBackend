using Microsoft.Extensions.DependencyInjection;

namespace Synq.Domain;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDomain(this IServiceCollection services)
    {
        return services;
    }
}