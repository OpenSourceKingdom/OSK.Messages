using OSK.Operations.Outputs.Models;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using OSK.Messages.Messaging.Models;
using OSK.Messages.Abstractions;

namespace OSK.Messages.Messaging.Ports;

/// <summary>
/// A central location for message delivery to be received in the messaging system
/// </summary>
public interface IMessageCenter
{
    /// <summary>
    /// Gets the recipient details for the registered delivery pipelines and boxes in the message center
    /// </summary>
    /// <returns>The collection of recipient details in the message center</returns>
    IEnumerable<MessageRecipientDetails> GetRecipientDetails();

    /// <summary>
    /// Initiates final message delivery by making the message center start processing a received message
    /// </summary>
    /// <typeparam name="TMessage">The type of message that was received</typeparam>
    /// <param name="message">The message that was received</param>
    /// <param name="cancellationToken">A token to cancel the operation</param>
    /// <returns>The result of the message delivery</returns>
    Task<Output> ReceiveAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default)
        where TMessage: IMessage;
}
