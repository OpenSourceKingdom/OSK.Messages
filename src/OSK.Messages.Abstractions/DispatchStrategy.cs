namespace OSK.Messages.Abstractions;

/// <summary>
/// Defines different methods of dispatching messages into the message system
/// </summary>
public enum DispatchStrategy
{
    /// <summary>
    /// Will dispatch a message to the first courier that is able to successfully sent
    /// </summary>
    FirstSuccess,

    /// <summary>
    /// Will attempt to dispatch a message to all couriers that are available
    /// </summary>
    Broadcast
}
