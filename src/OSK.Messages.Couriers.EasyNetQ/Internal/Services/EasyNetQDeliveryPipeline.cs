
using EasyNetQ;
using OSK.Messages.Abstractions;
using System;
using System.Threading;
using System.Threading.Tasks;
using IMessage = OSK.Messages.Abstractions.IMessage;
using OSK.Messages.Couriers.EasyNetQ.Models;
using OSK.Messages.Messaging.Ports;
using Microsoft.Extensions.Logging;

namespace OSK.Messages.Couriers.EasyNetQ.Internal.Services;

internal partial class EasyNetQDeliveryPipeline<TMessage>(string subscriberId, Action<ISubscriptionConfiguration> configurator, IBus bus, IMessageCenter messageCenter,
    ILogger<EasyNetQDeliveryPipeline<TMessage>> logger) : IMessageDeliveryPipeline
    where TMessage : IMessage
{
    #region Variables

    private IDisposable? _subscription;

    #endregion

    #region IMessageDeliveryService

    public string Id => subscriberId;

    public CourierName CourierName => EasyNetQCourierDescriptor.EasyNetQ;

    public Task StartAsync(CancellationToken cancellationToken = default)
    {
        if (_subscription != null)
        {
            return Task.CompletedTask;
        }

        try
        {
            _subscription = bus.PubSub.SubscribeAsync<TMessage>(
                subscriberId,
                async (msg, ct) =>
                {
                    try
                    {
                        await messageCenter.ReceiveAsync(msg, ct).ConfigureAwait(false);
                    }
                    catch (OperationCanceledException)
                    {
                    }
                    catch (Exception)
                    {
                    }
                }, configurator);
        }
        catch (Exception ex)
        {
            LogPipelineStartupError(logger, typeof(TMessage).FullName, ex);
        }

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _subscription?.Dispose();
        }
        catch (Exception ex)
        {
            LogPipelineStopError(logger, typeof(TMessage).FullName, ex);
        }
        finally
        {
            _subscription = null;
        }

        return Task.CompletedTask;
    }

    public async ValueTask DisposeAsync()
    {
        await StopAsync().ConfigureAwait(false);
        _subscription?.Dispose();
        _subscription = null;
    }

    #endregion

    #region Logging

    [LoggerMessage(eventId: 1, level: LogLevel.Error, "There was an error starting the EasyNetQPipeline for message type '{messageTypeName}'.")]
    private static partial void LogPipelineStartupError(ILogger logger, string messageTypeName, Exception ex);

    [LoggerMessage(eventId: 2, level: LogLevel.Error, "There was an error stopping the EasyNetQPipeline for message type '{messageTypeName}'.")]
    private static partial void LogPipelineStopError(ILogger logger, string messageTypeName, Exception ex);

    #endregion
}
