using OSK.Messages.Abstractions;
using OSK.Operations.Outputs.Models;
using System;
using System.Threading.Tasks;

namespace OSK.Messages.Messaging.Internal.Services;

internal class MessageFunctionRecipient<TMessage>(Func<MessageContext, TMessage, Task<Output>> handler) : MessageRecipient<TMessage>
    where TMessage : IMessage
{
    #region MessageRecipient Overrides

    protected override Task<Output> ReceiveAsync(MessageContext context, TMessage message)
        => handler(context, message);

    #endregion
}
