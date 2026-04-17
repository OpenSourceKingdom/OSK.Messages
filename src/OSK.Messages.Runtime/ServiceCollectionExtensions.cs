using Microsoft.Extensions.DependencyInjection;
using OSK.Messages.Runtime.Internal.Services;

namespace OSK.Messages.Runtime;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Addds the needed runtime services so that message receiving and delivery can happen in the background
    /// </summary>
    /// <param name="services">The service collection the services will be added to</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddMessageRuntime(this IServiceCollection services)
    {
        services.AddHostedService<HostedMessageDeliveryRuntime>();
        return services;
    }
}
