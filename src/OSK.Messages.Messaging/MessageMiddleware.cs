using OSK.Messages.Abstractions;
using OSK.Messages.Messaging.Models;
using OSK.Messages.Messaging.Ports;
using OSK.Operations.Outputs.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OSK.Messages.Messaging;

public abstract class MessageMiddleware<TMessage>: IMessageMiddleware
    where TMessage: IMessage
{
    #region IMessageMiddleware

    public Task<Output> HandleAsync(MessageContext context, MessageDelegate next)
        => context.Message is TMessage message
            ? HandleAsync(context, message, next)
            : next(context);

    #endregion

    #region Helpers

    protected abstract Task<Output> HandleAsync(MessageContext context, TMessage message, MessageDelegate next);

    #endregion
}
