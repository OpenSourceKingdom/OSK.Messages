using OSK.Hexagonal.MetaData;
using OSK.Operations.Outputs.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace OSK.Messages.Abstractions;

[HexagonalIntegration(HexagonalIntegrationType.IntegrationRequired)]
public interface ICourierService
{
    Task<Output> ScheduleAsync<TMessage>(TMessage message, TimeSpan delay, CancellationToken cancellationToken = default)
        where TMessage: IMessage;

    Task<Output> DeliverAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default)
        where TMessage: IMessage;
}
