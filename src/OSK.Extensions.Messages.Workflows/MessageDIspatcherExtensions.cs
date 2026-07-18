using OSK.Messages.Abstractions;
using OSK.Operations.Outputs.Models;
using OSK.Operations.Workflows.Models;
using OSK.Operations.Workflows.Tasks.Async;

namespace OSK.Extensions.Messages.Workflows;

public static class MessageDIspatcherExtensions
{
    /// <summary>
    /// Provides access to a non-async iterative task for dispatching a message when async is not usable
    /// </summary>
    /// <typeparam name="TMessage">The message to send</typeparam>
    /// <param name="dispatcher">The dispatcher to send the message</param>
    /// <param name="message">The message to send</param>
    /// <returns>An operation for the dispatch</returns>
    public static ITaskOperation<Output> DispatchMessage<TMessage>(this IMessageDispatcher dispatcher, TMessage message)
        where TMessage : IMessage
        => dispatcher.DispatchAsync(message).ToOperation();
}
