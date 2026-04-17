using System;
using OSK.Messages.Abstractions;

namespace OSK.Messages.Messaging.Models;

/// <inheritdoc/>
public class MessageContext<TMessage>(TMessage message, IServiceProvider services): MessageContext(message, services)
    where TMessage: IMessage
{
    #region Variables

    /// <summary>
    /// The typed message associated to the message delivery
    /// </summary>
    public new TMessage Message { get; } = message;

    #endregion
}
