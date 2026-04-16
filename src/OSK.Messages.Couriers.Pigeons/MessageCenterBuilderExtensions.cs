using OSK.Messages.Abstractions;
using OSK.Messages.Messaging.Ports;
using System;

namespace OSK.Messages.Couriers.Pigeons;

public static class MessageCenterBuilderExtensions
{
    public static IMessageCenterBuilder AddPigeon<TMessage>(this IMessageCenterBuilder builder, Action<IMessageBoxConfigurator<TMessage>> configurator)
        where TMessage: IMessage
    {
        builder.AddMessageBox(configurator);

        return builder;
    }
}
