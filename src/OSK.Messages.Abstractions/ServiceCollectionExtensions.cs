using Microsoft.Extensions.DependencyInjection;

namespace OSK.Messages.Abstractions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds a provided courier descriptor to the list of couriers able to be used by the <see cref="IMessageDispatcher"/>
    /// </summary>
    /// <typeparam name="TCourier">The type of courier</typeparam>
    /// <param name="services">The service collection that the descriptor will be added to</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddCourier<TCourier>(this IServiceCollection services)
        where TCourier : class, ICourierDescriptor
    {
        services.AddSingleton<ICourierDescriptor, TCourier>();

        return services;
    }
}
