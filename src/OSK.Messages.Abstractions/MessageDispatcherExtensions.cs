using OSK.Operations.Outputs.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace OSK.Messages.Abstractions;

public static class MessageDispatcherExtensions
{
    /// <summary>
    /// Attempts to dispatch a message using no delay and default dispatch options
    /// </summary>
    /// <param name="dispatcher">The dispatcher to send the message</param>
    /// <param name="message">The message to send</param>
    /// <param name="cancellationToken">A token to cancel the operation</param>
    /// <returns>The result of the message being dispatch</returns>
    public static Task<Output> DispatchAsync(this IMessageDispatcher dispatcher, IMessage message, CancellationToken cancellationToken = default)
        => dispatcher.DispatchAsync(message, TimeSpan.Zero, DispatchOptions.Default, cancellationToken);

    /// <summary>
    /// Attempts to dispatch a message using a provided delay with default dispatch options
    /// </summary>
    /// <param name="dispatcher">The dispatcher to send the message</param>
    /// <param name="message">The message to send</param>
    /// <param name="delay">The desired delay for the message</param>
    /// <param name="cancellationToken">A token to cancel the operation</param>
    /// <returns>The result of the message being dispatch</returns>
    public static Task<Output> DispatchAsync(this IMessageDispatcher dispatcher, IMessage message, TimeSpan delay, CancellationToken cancellationToken = default)
        => dispatcher.DispatchAsync(message, delay, DispatchOptions.Default, cancellationToken);
}
