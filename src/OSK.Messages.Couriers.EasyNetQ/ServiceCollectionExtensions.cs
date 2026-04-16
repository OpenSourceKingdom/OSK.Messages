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
    public static IServiceCollection AddEasyNetCourier(this IServiceCollection services, Action<ISubscriptionConfiguration> defaultSubscriberConfiguration)
        => services.AddEasyNetCourier(configurator =>
        {
            configurator.UseDefaultConfiguration(defaultSubscriberConfiguration);
        });

    public static IServiceCollection AddEasyNetCourier(this IServiceCollection services, Action<IEasyNetQCourierConfigurator> configurator)
    {
        if (configurator is null) 
        {
            throw new ArgumentNullException(nameof(configurator));
        }

        services.AddCourier<EasyNetQCourierDescriptor>();
        services.AddTransient<EasyNetQCourierService>();

        services.AddTransient(serviceProvider =>
        {
            var easyNetQConfigurator = new EasyNetQConfigurator();
            configurator(easyNetQConfigurator);

            return easyNetQConfigurator.BuildFactory(serviceProvider);
        });

        return services;
    }
}
