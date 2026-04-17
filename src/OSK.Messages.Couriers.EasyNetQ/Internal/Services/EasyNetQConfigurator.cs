using EasyNetQ;
using Microsoft.Extensions.DependencyInjection;
using OSK.Messages.Couriers.EasyNetQ.Ports;
using OSK.Messages.Messaging.Naming;
using OSK.Messages.Messaging.Naming.TypeName;
using OSK.Messages.Messaging.Ports;
using System;
using System.Collections.Generic;

namespace OSK.Messages.Couriers.EasyNetQ.Internal.Services;

internal class EasyNetQConfigurator : IEasyNetQCourierConfigurator
{
    #region Variables

    private Action<ISubscriptionConfiguration>? _defaultSubscriberConfiguration;
    private IPipelineNamingStrategy? _namingStrategy;

    private readonly Dictionary<Type, Action<ISubscriptionConfiguration>> _messageSubscriptions = [];

    #endregion

    #region IEasyNetQConfigurator

    public IEasyNetQCourierConfigurator UseDefaultConfiguration(Action<ISubscriptionConfiguration> configurator)
    {
        if (configurator is null)
        {
            throw new ArgumentNullException(nameof(configurator));
        }

        _defaultSubscriberConfiguration = configurator;
        return this;
    }

    public IEasyNetQCourierConfigurator UseConfiguration<TMessage>(Action<ISubscriptionConfiguration> configurator) 
        where TMessage : IMessage
    {
        if (configurator is null)
        {
            throw new ArgumentNullException(nameof(configurator));
        }

        _messageSubscriptions[typeof(TMessage)] = configurator;
        return this;
    }

    public IEasyNetQCourierConfigurator UseNamingStrategy(IPipelineNamingStrategy strategy)
    {
        if (strategy is null)
        {
            throw new ArgumentNullException(nameof(strategy));
        }

        _namingStrategy = strategy;
        return this;
    }

    #endregion

    #region Helpers

    public IMessageDeliveryPipelineFactory BuildFactory(IServiceProvider services)
    {
        if (_defaultSubscriberConfiguration is null)
        {
            throw new InvalidOperationException("The default subscriber configuration must be set.");
        }

        var namingStrategy = _namingStrategy ?? new PipelineTypeNamingStrategy();
        return ActivatorUtilities.CreateInstance<EasyNetQPipelineFactory>(services, [ _defaultSubscriberConfiguration, _messageSubscriptions,  namingStrategy]);
    }

    #endregion
}
