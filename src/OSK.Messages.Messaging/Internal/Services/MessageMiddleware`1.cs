using OSK.Messages.Abstractions;
using System;
using System.Threading.Tasks;
using OSK.Messages.Messaging.Models;
using OSK.Messages.Messaging.Ports;

namespace OSK.Messages.Messaging.Internal.Services;

internal class MessageFunctionMiddleware<TMessage>(Func<MessageContext, TMessage, MessageDelegate, Task> middleware) : IMessageMiddleware
    where TMessage : IMessage
{
    #region IMessageMiddleware

    public Task HandleAsync(MessageContext context, MessageDelegate next)
        => context.Message is TMessage message
            ? middleware(context, message, next)
            : next(context);

    #endregion
}
