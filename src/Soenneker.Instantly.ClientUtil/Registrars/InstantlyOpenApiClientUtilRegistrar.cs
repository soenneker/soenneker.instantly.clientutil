using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Soenneker.Instantly.Client.Registrars;
using Soenneker.Instantly.ClientUtil.Abstract;

namespace Soenneker.Instantly.ClientUtil.Registrars;

/// <summary>
/// Registers the lazily created Instantly generated-client provider.
/// </summary>
public static class InstantlyOpenApiClientUtilRegistrar
{
    /// <summary>
    /// Adds <see cref="InstantlyOpenApiClientUtil"/> as a singleton service. <para/>
    /// </summary>
    public static IServiceCollection AddInstantlyOpenApiClientUtilAsSingleton(this IServiceCollection services)
    {
        services.AddInstantlyClientAsSingleton().TryAddSingleton<IInstantlyOpenApiClientUtil, InstantlyOpenApiClientUtil>();

        return services;
    }

    /// <summary>
    /// Adds <see cref="InstantlyOpenApiClientUtil"/> as a scoped service. <para/>
    /// </summary>
    public static IServiceCollection AddInstantlyOpenApiClientUtilAsScoped(this IServiceCollection services)
    {
        services.AddInstantlyClientAsSingleton().TryAddScoped<IInstantlyOpenApiClientUtil, InstantlyOpenApiClientUtil>();

        return services;
    }
}
