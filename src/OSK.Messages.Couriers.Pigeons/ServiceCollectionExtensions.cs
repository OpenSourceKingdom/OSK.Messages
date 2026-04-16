using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using OSK.Messages.Couriers.Pigeons.Internal.Services;
using OSK.Messages.Couriers.Pigeons.Models;
using OSK.Messages.Couriers.Pigeons.Options;
using OSK.Messages.Couriers.Pigeons.Ports;
using OSK.Messages.Messaging;
using System;

namespace OSK.Messages.Couriers.Pigeons;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCourierPigeons(this IServiceCollection services)
        => services.AddCourierPigeons(_ => { });

    public static IServiceCollection AddCourierPigeons(this IServiceCollection services, Action<PigeonOptions> configurator)
    {
        if (configurator is null)
        {
            throw new ArgumentNullException(nameof(configurator));
        }

        services.Configure(configurator);
        
        services.AddCourier<CourierPigeonDescriptor>();
        services.TryAddTransient<IPigeonHold, PigeonHold>();

        return services;
    }
}
