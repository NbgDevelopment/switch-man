using Microsoft.Extensions.DependencyInjection;
using NbgDev.SwitchMan.Switches.Contract;

namespace NbgDev.SwitchMan.Switches.TLSG2008;

/// <summary>
/// Extension methods for registering switch access services
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds TL-SG2008 switch access service to the dependency injection container
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddTlSg2008SwitchAccess(this IServiceCollection services)
    {
        services.AddSingleton<ISwitchAccessService, TlSg2008SwitchAccessService>();
        return services;
    }
}
