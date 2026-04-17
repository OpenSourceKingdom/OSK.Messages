using System;

namespace OSK.Messages.Messaging.Options;

/// <summary>
/// Provides configuration options for the underlying processing within the message delivery system
/// </summary>
public class MessagingOptions
{
    #region Variables

    /// <summary>
    /// Determines if messages that are inherited should be delivered to in addition to the expected type. For example, if this is of and you have two types, T and T:U. If this is false, then the outcome of 
    /// message delivery of a message of T or U would only go to boxes for type T or U, but if this is set to true, sending messages of type T will trigger delviery for boxes that handle T or U
    /// </summary>
    public bool AllowInheritedMessageDelivery { get; set; }

    #endregion
}
