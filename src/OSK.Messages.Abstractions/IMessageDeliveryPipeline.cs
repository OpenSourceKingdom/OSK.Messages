using OSK.Hexagonal.MetaData;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace OSK.Messages.Abstractions;

/// <summary>
/// Represents a delivery pipeline that needs to run in the background in order to process final delivery of received messages
/// </summary>
[HexagonalIntegration(HexagonalIntegrationType.IntegrationOptional)]
public interface IMessageDeliveryPipeline: IAsyncDisposable
{
    /// <summary>
    /// A unique id for the pipeline in the courier service
    /// </summary>
    string Id { get; }

    /// <summary>
    /// The strongly-typed named courier the pipeline belongs to
    /// </summary>
    public CourierName CourierName { get; }

    /// <summary>
    /// Activates the pipeline to start processing message deliveries
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation</param>
    /// <returns>A task for the operation</returns>
    Task StartAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Deactivates the pipepline to stop processing message deliveries
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation</param>
    /// <returns>A task for the operation</returns>
    Task StopAsync(CancellationToken cancellationToken = default);
}
