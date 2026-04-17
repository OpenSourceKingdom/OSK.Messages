using EasyNetQ;
using OSK.Hexagonal.MetaData;
using OSK.Messages.Messaging.Naming;
using System;

namespace OSK.Messages.Couriers.EasyNetQ.Ports;

/// <summary>
/// A configurator that helps to setup the EasyNetQ queues
/// </summary>
[HexagonalIntegration(HexagonalIntegrationType.LibraryProvided)]
public interface IEasyNetQCourierConfigurator
{
    /// <summary>
    /// Sets the default configuration used to integrate new queues
    /// </summary>
    /// <param name="configurator">The default configuration action for queues</param>
    /// <returns>The configurator for chaining</returns>
    IEasyNetQCourierConfigurator UseDefaultConfiguration(Action<ISubscriptionConfiguration> configurator);

    /// <summary>
    /// Sets a custom typed configurated used to integrate queues of type <typeparamref name="TMessage"/>"/>
    /// </summary>
    /// <typeparam name="TMessage">The specific message type the configuration will apply to</typeparam>
    /// <param name="configurator">The configuration action for the queue</param>
    /// <returns>The configurator for chaining</returns>
    IEasyNetQCourierConfigurator UseConfiguration<TMessage>(Action<ISubscriptionConfiguration> configurator)
        where TMessage : IMessage;

    /// <summary>
    /// Sets a specific naming strategy to use for new EasyNetQ pipelines that are created
    /// </summary>
    /// <param name="strategy">The <see cref="IPipelineNamingStrategy"/> that will be used</param>
    /// <returns>The configurator for chaining</returns>
    IEasyNetQCourierConfigurator UseNamingStrategy(IPipelineNamingStrategy strategy);
}
