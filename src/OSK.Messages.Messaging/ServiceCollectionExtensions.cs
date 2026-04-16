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
    public static IServiceCollection AddCourier<TCourier>(this IServiceCollection services)
        where TCourier: class, ICourierDescriptor
    {
        services.AddSingleton<ICourierDescriptor, TCourier>();

        return services;
    }

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
