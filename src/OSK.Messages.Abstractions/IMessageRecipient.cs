using System;
using System.Threading.Tasks;

namespace OSK.Messages.Abstractions;

/// <summary>
/// Represents an object that can receive and handle messages within the messaging system
/// </summary>
public interface IMessageRecipient
{
    /// <summary>
    /// Describes the type of message that this recipient can process
    /// </summary>
    Type MessageType { get; }

    /// <summary>
    /// Processes the message
    /// </summary>
    /// <param name="context">The contextual information for the message that was delivered</param>
    /// <returns>An output for the result of the delivery</returns>
    Task ReceiveAsync(MessageContext context);
}
