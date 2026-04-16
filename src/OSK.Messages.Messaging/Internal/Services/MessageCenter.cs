using OSK.Messages.Abstractions;
using OSK.Messages.Messaging.Options;
using OSK.Operations.Outputs;
using OSK.Operations.Outputs.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using OSK.Messages.Messaging.Models;
using OSK.Messages.Messaging.Ports;

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

    public async Task<Output> ReceiveAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default)
        where TMessage: IMessage
    {
        if (!_messageBoxLookup.TryGetValue(typeof(TMessage), out var box))
        {
            return Out.Success();
        }

        var messageBoxes = GetMessageBoxes<TMessage>();
        var outputs = new List<Output>();

        foreach (var messageBox in messageBoxes)
        {
            var output = await messageBox.DeliverMessageAsync(message, services);
            outputs.Add(GetCalculatedOutput(output));
        }

        var messageMultiOutput = Out.Multiple(outputs);
        return GetCalculatedOutput(messageMultiOutput);
    }

    #endregion

    #region Helpers

    private Output GetCalculatedOutput(Output output)
        => output switch
        {
            MultiOutput multiOutput => multiOutput.StatusCode.Status == OutputStatus.MultiStatus
                ? multiOutput.GetErrorOutputs().Any() ? Out.Success() : Out.Status(OutputStatus.InternalError)
                : Out.Status(multiOutput.StatusCode.Status),
            _ => output
        };

    private MessageBox[] GetMessageBoxes<TMessage>()
    {
        var messageType = typeof(TMessage);
        if (_messageBoxLookup.TryGetValue(messageType, out var validBoxes))
        {
            return validBoxes;
        }

        validBoxes = options.AllowInheritedRecipients
            ? [.. messageBoxes.Where(box => box.BoxType.IsAssignableFrom(messageType))]
            : [.. messageBoxes.Where(box => box.BoxType == messageType)];

        _messageBoxLookup[messageType] = validBoxes;
        return validBoxes;
    }

    #endregion
}
