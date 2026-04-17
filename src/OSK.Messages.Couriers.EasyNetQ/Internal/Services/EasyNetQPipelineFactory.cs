using EasyNetQ;
using Microsoft.Extensions.DependencyInjection;
using OSK.Messages.Messaging.Models;
using OSK.Messages.Messaging.Naming;
using OSK.Messages.Messaging.Ports;
using System;
using System.Collections.Generic;

namespace OSK.Messages.Couriers.EasyNetQ.Internal.Services;

internal class EasyNetQPipelineFactory(Action<ISubscriptionConfiguration> defaultConfigurator, Dictionary<Type, Action<ISubscriptionConfiguration>> typedConfigurators,
    IPipelineNamingStrategy namingStrategy, IServiceProvider services) : IMessageDeliveryPipelineFactory
{
    #region IMessageDeliveryPipelineFactory

    public IMessageDeliveryPipeline CreatePipeline(MessageRecipientDetails details)
    {
        var pipelineType = typeof(EasyNetQDeliveryPipeline<>).MakeGenericType(details.MessageType);
        var subscriberId = namingStrategy.GetPipelineName(details);

        var configurator = typedConfigurators.TryGetValue(details.MessageType, out var typedConfigurator)
            ? typedConfigurator
            : defaultConfigurator;

        return (IMessageDeliveryPipeline)ActivatorUtilities.CreateInstance(services, pipelineType, [subscriberId, configurator]);
    }

    #endregion
}
