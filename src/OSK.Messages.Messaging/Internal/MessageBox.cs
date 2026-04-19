using OSK.Messages.Abstractions;
using OSK.Operations.Outputs;
using OSK.Operations.Outputs.Models;
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

    public async Task<Output> DeliverMessageAsync(MessageContext context)
    {
        var outputs = new List<Output>();
        foreach (var messageDelegate in recipients.Select(recipient => recipient.Delegate))
        {
            var output = await messageDelegate(context);
            outputs.Add(output);
        }

        return Out.Multiple(outputs);
    }
}
