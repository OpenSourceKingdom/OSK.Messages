using EasyNetQ;
using Microsoft.Extensions.DependencyInjection;
using OSK.Messages.Couriers.EasyNetQ.Internal.Services;
using OSK.Messages.Couriers.EasyNetQ.Models;
using OSK.Messages.Couriers.EasyNetQ.Ports;
using OSK.Messages.Messaging;
using System;

namespace OSK.Messages.Couriers.EasyNetQ;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the EasyNetQ courier to the messaging system with a default queue configuration to apply to all queues
    /// </summary>
    /// <param name="services">The service collection to add the dependencies to</param>
    /// <param name="defaultSubscriberConfiguration">The default queue configuration to apply to all queues</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddEasyNetCourier(this IServiceCollection services, Action<ISubscriptionConfiguration> defaultSubscriberConfiguration)
        => services.AddEasyNetCourier(configurator =>
        {
            configurator.UseDefaultConfiguration(defaultSubscriberConfiguration);
        });

    /// <summary>
    /// Adds the EasyNetQ courier to the messaging system with a provided <see cref="IEasyNetQCourierConfigurator"/> action 
    /// </summary>
    /// <param name="services">The service collection to add the dependencies to</param>
    /// <param name="configurator">The action to configure the courier</param>
    /// <returns>The service collection for chaining</returns>
    /// <exception cref="ArgumentNullException">If the configurator action is null</exception>
    public static IServiceCollection AddEasyNetCourier(this IServiceCollection services, Action<IEasyNetQCourierConfigurator> configurator)
    {
        if (configurator is null) 
        {
            throw new ArgumentNullException(nameof(configurator));
        }

        services.AddCourier<EasyNetQCourierDescriptor>();
        services.AddTransient<IEasyNetQCourierService, EasyNetQCourierService>();

        services.AddTransient(serviceProvider =>
        {
            var easyNetQConfigurator = new EasyNetQConfigurator();
            configurator(easyNetQConfigurator);

            return easyNetQConfigurator.BuildFactory(serviceProvider);
        });

        return services;
    }
}
