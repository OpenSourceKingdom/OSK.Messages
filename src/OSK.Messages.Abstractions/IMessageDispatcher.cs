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
    /// <param name="message"></param>
    /// <param name="delay"></param>
    /// <param name="options"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Output> DispatchAsync(IMessage message, TimeSpan delay, DispatchOptions options, CancellationToken cancellationToken = default);
}
