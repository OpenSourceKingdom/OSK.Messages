using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using OSK.Messages.Abstractions;
using OSK.Messages.Messaging.Internal.Services;
using OSK.Messages.Messaging.Models;
using OSK.Messages.Messaging.Ports;
using System;

namespace OSK.Messages.Messaging;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds a provided courier descriptor to the list of couriers able to be used by the <see cref="IMessageDispatcher"/>
    /// </summary>
    /// <typeparam name="TCourier">The type of courier</typeparam>
    /// <param name="services">The service collection that the descriptor will be added to</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddCourier<TCourier>(this IServiceCollection services)
        where TCourier: class, ICourierDescriptor
    {
        services.AddSingleton<ICourierDescriptor, TCourier>();

        return services;
    }

    /// <summary>
    /// Adds the messaging system to the service collection so messages can be dispatched, received, and delivered
    /// </summary>
    /// <param name="services">The service collection that the messaging services will be added to</param>
    /// <param name="configurator">A configuration action for the messaging system setup</param>
    /// <returns>The service collection for chaining</returns>
    /// <exception cref="ArgumentNullException">if the configurator is null</exception>
    public static IServiceCollection AddMessaging(this IServiceCollection services, Action<IMessageCenterBuilder> configurator)
    {
        if (configurator is null)
        {
            throw new ArgumentNullException(nameof(configurator));
        }

        services.TryAddTransient<IMessageDispatcher, MessageDispatcher>();

        services.TryAddSingleton(serviceProvider =>
        {
            var builder = new MessageCenterBuilder();
            configurator(builder);

            return builder.BuildMessageCenter(serviceProvider);
        });

        return services;
    }
}
