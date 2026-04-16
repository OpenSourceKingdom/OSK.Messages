using System;
using OSK.Messages.Abstractions;

namespace OSK.Messages.Messaging.Models;

public class MessageContext(IMessage message, IServiceProvider services)
{
    #region Variables

    public IMessage Message { get; } = message;

    public IServiceProvider Services { get; } = services;

    #endregion
}
