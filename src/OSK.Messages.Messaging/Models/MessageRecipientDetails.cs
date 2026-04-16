using System;

namespace OSK.Messages.Messaging.Models;

public class MessageRecipientDetails(Type messageType, Type recipientType)
{
    public Type MessageType { get; } = messageType;

    public Type RecipientType { get; } = recipientType;
}
