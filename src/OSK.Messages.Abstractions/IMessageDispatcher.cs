using OSK.Hexagonal.MetaData;
using OSK.Operations.Outputs.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace OSK.Messages.Abstractions;

[HexagonalIntegration(HexagonalIntegrationType.LibraryProvided, HexagonalIntegrationType.ConsumerPointOfEntry)]
public interface IMessageDispatcher
{
    Task<Output> DispatchAsync(IMessage message, TimeSpan delay, DispatchOptions options, CancellationToken cancellationToken = default);
}
