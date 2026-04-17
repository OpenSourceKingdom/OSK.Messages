using OSK.Hexagonal.MetaData;
using OSK.Operations.Outputs.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace OSK.Messages.Abstractions;

/// <summary>
/// A service that is capable of dispatching a message and handling the process of delivery to a message center
/// </summary>
[HexagonalIntegration(HexagonalIntegrationType.IntegrationRequired)]
public interface ICourierService
{
    /// <summary>
    /// Informs the courier to schedule a message to be delivered at a later time
    /// </summary>
    /// <typeparam name="TMessage">The type of <see cref="IMessage"/> being scheduled</typeparam>
    /// <param name="message">The message being scheduled</param>
    /// <param name="delay">The <see cref="TimeSpan"/> delay for the message</param>
    /// <param name="cancellationToken">A token to cancel the operation</param>
    /// <returns>An output representing the status of the scheduling attempt</returns>
    Task<Output> ScheduleAsync<TMessage>(TMessage message, TimeSpan delay, CancellationToken cancellationToken = default)
        where TMessage: IMessage;

    /// <summary>
    /// Informs the courier to immediately deliver a message
    /// </summary>
    /// <typeparam name="TMessage">The type of <see cref="IMessage"/> being delivered</typeparam>
    /// <param name="message">The message being delivered</param>
    /// <param name="cancellationToken">A token to cancel the operation</param>
    /// <returns>An output representing the status of the delivery attempt</returns>
    Task<Output> DeliverAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default)
        where TMessage: IMessage;
}
