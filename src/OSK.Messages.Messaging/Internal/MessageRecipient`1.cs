using OSK.Messages.Abstractions;
using OSK.Messages.Messaging.Models;
using OSK.Messages.Messaging.Ports;

namespace OSK.Messages.Messaging.Internal;

internal class MessageBoxRecipient<TMessage>(IMessageHandler<TMessage> handler, MessageDelegate messageDelegate)
    where TMessage: IMessage
{
    public IMessageHandler<TMessage> Handler => handler;

    public MessageDelegate Delegate => messageDelegate;
}
