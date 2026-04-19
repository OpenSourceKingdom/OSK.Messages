using System;

namespace OSK.Messages.Abstractions;

/// <summary>
/// Provides details to a message box and recipient within the message center
/// </summary>
/// <param name="messageType">The type of message that is read</param>
/// <param name="recipientType">The type of recipient that handles the message</param>
public class MessageRecipientDetails(Type messageType, Type recipientType)
{
    /// <summary>
    /// The type of message that is handled by the recipient
    /// </summary>
    public Type MessageType { get; } = messageType;

    /// <summary>
    /// The type of recipient that handles a message
    /// </summary>
    public Type RecipientType { get; } = recipientType;
}
