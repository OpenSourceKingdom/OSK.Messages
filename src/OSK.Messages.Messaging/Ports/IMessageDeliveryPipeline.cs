using System;
using System.Threading;
using System.Threading.Tasks;
using OSK.Messages.Abstractions;

namespace OSK.Messages.Messaging.Ports;

public interface IMessageDeliveryPipeline: IAsyncDisposable
{
    string Id { get; }

    public CourierName CourierName { get; }

    Task StartAsync(CancellationToken cancellationToken = default);

    Task StopAsync(CancellationToken cancellationToken = default);
}
