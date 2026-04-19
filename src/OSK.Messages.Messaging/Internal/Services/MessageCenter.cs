using OSK.Messages.Abstractions;
using OSK.Messages.Messaging.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OSK.Messages.Messaging.Internal.Services;

internal partial class MessageCenter(IList<MessageBox> messageBoxes, IServiceProvider services, MessagingOptions options) : IMessageCenter
{
    #region Variables

    private readonly Dictionary<Type, MessageBox[]> _messageBoxLookup = [];

    #endregion

    #region IMessageCenter

    public IEnumerable<MessageRecipientDetails> GetRecipientDetails()
    {
        return messageBoxes.SelectMany(box => box.GetRecipientDetails());
    }

    public async Task ReceiveAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default)
        where TMessage: IMessage
    {
        var messageBoxes = GetMessageBoxes<TMessage>();
        if (messageBoxes.Length is 0)
        {
            return;
        }

        var messageContext = new MessageContext(message, services);
        foreach (var messageBox in messageBoxes)
        {
            await messageBox.DeliverMessageAsync(messageContext);
        }
    }

    #endregion

    #region Helpers

    private MessageBox[] GetMessageBoxes<TMessage>()
    {
        var messageType = typeof(TMessage);
        if (_messageBoxLookup.TryGetValue(messageType, out var validBoxes))
        {
            return validBoxes;
        }

        validBoxes = options.AllowInheritedMessageDelivery
            ? [.. messageBoxes.Where(box => box.BoxType.IsAssignableFrom(messageType))]
            : [.. messageBoxes.Where(box => box.BoxType == messageType)];

        _messageBoxLookup[messageType] = validBoxes;
        return validBoxes;
    }

    #endregion
}
