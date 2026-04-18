using OSK.Messages.Abstractions;
using OSK.Operations.Outputs.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OSK.Messages.Messaging.Internal;

internal abstract class MessageBox(Type boxType)
{
    public Type BoxType { get; } = boxType;

    public abstract IEnumerable<MessageRecipientDetails> GetRecipientDetails();

    public abstract Task<Output> DeliverMessageAsync(IMessage message, IServiceProvider services);
}
