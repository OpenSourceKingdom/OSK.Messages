using OSK.Messages.Abstractions;
using OSK.Operations.Outputs.Models;
using System;
using System.Threading.Tasks;
using OSK.Messages.Messaging.Models;
using OSK.Messages.Messaging.Ports;

namespace OSK.Messages.Messaging.Internal.Services;

internal class MessageRecipient<TMessage>(Func<MessageContext<TMessage>, Task<Output>> handler) : IMessageRecipient<TMessage>
    where TMessage : IMessage
{
    #region IMessageRecipient

    public Task<Output> HandleAsync(MessageContext<TMessage> context)
        => handler(context);

    #endregion
}
