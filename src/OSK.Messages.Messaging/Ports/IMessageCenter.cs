using OSK.Operations.Outputs.Models;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using OSK.Messages.Messaging.Models;
using OSK.Messages.Abstractions;

namespace OSK.Messages.Messaging.Ports;

public interface IMessageCenter
{
    IEnumerable<MessageRecipientDetails> GetRecipientDetails();

    Task<Output> ReceiveAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default)
        where TMessage: IMessage;
}
