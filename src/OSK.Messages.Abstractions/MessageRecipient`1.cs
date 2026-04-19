using OSK.Operations.Outputs;
using System;
using System.Threading.Tasks;

namespace OSK.Messages.Abstractions;

/// <summary>
/// A typed message recipient for receiving messages of a specific type
/// </summary>
/// <typeparam name="TMessage"></typeparam>
public abstract class MessageRecipient<TMessage> : IMessageRecipient
    where TMessage : IMessage
{
    #region IMessageRecipient

    public Type MessageType => typeof(TMessage);

    public Task ReceiveAsync(MessageContext context)
        => context.Message is TMessage message
            ? ReceiveAsync(context, message)
            : Task.FromResult(Out.Success());

    #endregion

    #region Helpers

    protected abstract Task ReceiveAsync(MessageContext context, TMessage message);

    #endregion
}
