using EasyNetQ;
using OSK.Messages.Messaging;
using System;

namespace OSK.Messages.Couriers.EasyNetQ.Ports;

public interface IEasyNetQCourierConfigurator
{
    IEasyNetQCourierConfigurator UseDefaultConfiguration(Action<ISubscriptionConfiguration> configurator);

    IEasyNetQCourierConfigurator UseConfiguration<TMessage>(Action<ISubscriptionConfiguration> configurator)
        where TMessage : IMessage;

    IEasyNetQCourierConfigurator UseNamingStrategy(IPipelineNamingStrategy strategy);
}
