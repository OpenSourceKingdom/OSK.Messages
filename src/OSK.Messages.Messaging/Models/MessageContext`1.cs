using System;
using OSK.Messages.Abstractions;

namespace OSK.Messages.Messaging.Models;

public class MessageContext<TMessage>(TMessage message, IServiceProvider services): MessageContext(message, services)
    where TMessage: IMessage
{
    #region Variables

    public new TMessage Message { get; } = message;

    #endregion
}
