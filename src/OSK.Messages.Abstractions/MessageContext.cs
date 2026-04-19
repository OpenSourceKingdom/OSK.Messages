using System;

namespace OSK.Messages.Abstractions;

/// <summary>
/// Provides some contextual information about the event
/// </summary>
/// <param name="message">The message that was sent</param>
/// <param name="services">The service provider associated to the message delivery</param>
public class MessageContext(IMessage message, IServiceProvider services)
{
    #region Variables

    /// <summary>
    /// The message that was sent
    /// </summary>
    public IMessage Message { get; } = message;

    /// <summary>
    /// The active services handling the message delivery
    /// </summary>
    public IServiceProvider Services { get; } = services;

    #endregion
}
