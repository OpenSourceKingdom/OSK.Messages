using OSK.Messages.Abstractions;
using OSK.Messages.Messaging.Models;
using OSK.Operations.Outputs;
using OSK.Operations.Outputs.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OSK.Messages.Messaging.Internal;

internal class MessageBox<TMessage>(MessageBoxRecipient<TMessage>[] recipients)
    : MessageBox(typeof(TMessage))
    where TMessage : IMessage
{
    #region MessageBox Overrides

    public override async Task<Output> DeliverMessageAsync(IMessage message, IServiceProvider services)
    {
        if (message is not TMessage typedMessage)
        {
            return Out.Success();
        }

        var messageContext = new MessageContext<TMessage>(typedMessage, services);
        var outputs = new List<Output>();

        foreach (var messageDelegate in recipients.Select(recipient => recipient.Delegate))
        {
            var output = await messageDelegate(messageContext);
            outputs.Add(output);
        }

        return Out.Multiple(outputs);
    }

    public override IEnumerable<MessageRecipientDetails> GetRecipientDetails()
        => recipients.Select(recipient => new MessageRecipientDetails(BoxType, recipient.Handler.GetType()));

    #endregion
}
