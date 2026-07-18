using OSK.Hexagonal.MetaData;
using OSK.Operations.Outputs.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace OSK.Messages.Abstractions;

/// <summary>
/// A central location to initiate the dispatching of messages
/// </summary>
[HexagonalIntegration(HexagonalIntegrationType.LibraryProvided, HexagonalIntegrationType.ConsumerPointOfEntry)]
public interface IMessageDispatcher
{
    /// <summary>
    /// Attempts to dispatch a message into the messaging system using the provided dispatch options and optional delay
    /// </summary>
    /// <param name="message">The message to send</param>
    /// <param name="delay">A delay in the message to dispatch</param>
    /// <param name="options">Extra options for sending the message</param>
    /// <param name="cancellationToken">The token to cancel the operation</param>
    /// <returns>An output for the dispatch operation</returns>
    Task<Output> DispatchAsync<TMessage>(TMessage message, TimeSpan delay, DispatchOptions options, CancellationToken cancellationToken = default)
        where TMessage: IMessage;
}
