using OSK.Messages.Abstractions;
using OSK.Messages.Messaging.Models;
using OSK.Messages.Messaging.Ports;
using OSK.Operations.Outputs.Models;
using System.Threading.Tasks;

namespace OSK.Messages.Messaging;

/// <summary>
/// Provides a middleware that is typed for a specific message type.
/// </summary>
/// <typeparam name="TMessage">The type of message the middleware is associated with</typeparam>
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
