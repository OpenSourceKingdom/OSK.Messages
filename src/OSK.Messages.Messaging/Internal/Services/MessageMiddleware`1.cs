using OSK.Messages.Abstractions;
using OSK.Operations.Outputs.Models;
using System;
using System.Threading.Tasks;
using OSK.Messages.Messaging.Models;
using OSK.Messages.Messaging.Ports;

namespace OSK.Messages.Messaging.Internal.Services;

internal class MessageMiddleware<TMessage>(Func<MessageContext<TMessage>, MessageDelegate<TMessage>, Task<Output>> middleware) : IMessageMiddleware<TMessage>
    where TMessage : IMessage
{
    #region IMessageMiddleware

    public Task<Output> HandleAsync(MessageContext<TMessage> context, MessageDelegate<TMessage> next)
        => middleware(context, next);

    #endregion
}
