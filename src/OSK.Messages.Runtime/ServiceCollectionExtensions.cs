using Microsoft.Extensions.DependencyInjection;
using OSK.Messages.Runtime.Internal.Services;

namespace OSK.Messages.Runtime;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMessageRuntime(this IServiceCollection services)
    {
        services.AddHostedService<HostedMessageDeliveryRuntime>();
        return services;
    }
}
