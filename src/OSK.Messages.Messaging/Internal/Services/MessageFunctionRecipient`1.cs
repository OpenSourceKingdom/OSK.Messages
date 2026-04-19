using OSK.Messages.Abstractions;
using System;
using System.Threading.Tasks;

namespace OSK.Messages.Messaging.Internal.Services;

internal class MessageFunctionRecipient<TMessage>(Func<MessageContext, TMessage, Task> handler) : MessageRecipient<TMessage>
    where TMessage : IMessage
{
    #region MessageRecipient Overrides

    protected override Task ReceiveAsync(MessageContext context, TMessage message)
        => handler(context, message);

    #endregion
}
