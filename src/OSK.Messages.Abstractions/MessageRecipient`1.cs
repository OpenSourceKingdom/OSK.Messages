using OSK.Operations.Outputs;
using OSK.Operations.Outputs.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OSK.Messages.Abstractions;

public abstract class MessageRecipient<TMessage> : IMessageRecipient
    where TMessage : IMessage
{
    #region IMessageRecipient

    public Type MessageType => typeof(TMessage);

    public Task<Output> ReceiveAsync(MessageContext context)
        => context.Message is TMessage message
            ? ReceiveAsync(context, message)
            : Task.FromResult(Out.Success());

    #endregion

    #region Helpers

    protected abstract Task<Output> ReceiveAsync(MessageContext context, TMessage message);

    #endregion
}
