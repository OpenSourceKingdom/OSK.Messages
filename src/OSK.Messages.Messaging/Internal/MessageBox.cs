using OSK.Messages.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OSK.Messages.Messaging.Internal;

internal class MessageBox(Type boxType, MessageBoxRecipient[] recipients)
{
    public Type BoxType { get; } = boxType;

    public IEnumerable<MessageRecipientDetails> GetRecipientDetails()
        => recipients.Select(recipient => new MessageRecipientDetails(BoxType, recipient.Handler.GetType()));

    public async Task DeliverMessageAsync(MessageContext context)
    {
        foreach (var messageDelegate in recipients.Select(recipient => recipient.Delegate))
        {
            await messageDelegate(context);
        }
    }
}
