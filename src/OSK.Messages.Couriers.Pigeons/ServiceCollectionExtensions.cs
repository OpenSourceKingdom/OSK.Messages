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
    /// <summary>
    /// Adds the courier pigeons to the dependency container using default pigeon options
    /// </summary>
    /// <param name="services">The service collection to add the depdencies to</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddCourierPigeons(this IServiceCollection services)
        => services.AddCourierPigeons(_ => { });

    /// <summary>
    /// Adds the courier pigeons to the dependency container using the provided <see cref="PigeonOptions"/> configurator
    /// </summary>
    /// <param name="services">The service collection to add the depdencies to</param>
    /// <param name="configurator">The action configuration to apply to the pigeon options</param>
    /// <returns>The service collection for chaining</returns>
    /// <exception cref="ArgumentNullException">If configurator is null</exception>
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
