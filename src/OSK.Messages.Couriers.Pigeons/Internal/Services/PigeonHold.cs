using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OSK.Messages.Abstractions;
using OSK.Messages.Couriers.Pigeons.Options;
using OSK.Messages.Couriers.Pigeons.Ports;
using OSK.Operations.Outputs;
using OSK.Operations.Outputs.Models;
using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using OSK.Messages.Messaging.Ports;

namespace OSK.Messages.Couriers.Pigeons.Internal.Services;

internal partial class PigeonHold(IMessageCenter messageCenter, IOptions<PigeonOptions> options, ILogger<PigeonHold> logger) : IPigeonHold
{
    #region IPigeonHold

    public Task<Output> DeliverAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default) 
        where TMessage : IMessage
    {
        if (!options.Value.UseBackgroundMessaging)
        {
            return messageCenter.ReceiveAsync(message, cancellationToken);
        }

        _ = Task.Run(async () =>
        {
            try
            {
                await messageCenter.ReceiveAsync(message, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                LogAbortedMessageOperation(logger);
            }
            catch (Exception ex)
            {
                LogMessageOperationError(logger, ex);
            }
        }, CancellationToken.None);

        return Task.FromResult(Out.Success());
    }

    public Task<Output> ScheduleAsync<TMessage>(TMessage message, TimeSpan delay, CancellationToken cancellationToken = default) 
        where TMessage : IMessage
    {
        _ = Task.Run(async () =>
        {
            try
            {
                await Task.Delay(delay, cancellationToken);
                await DeliverAsync(message, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                LogAbortedMessageOperation(logger);
            }
            catch (Exception ex)
            {
                LogMessageOperationError(logger, ex);
            }
        }, CancellationToken.None);

        return Task.FromResult(Out.Success());
    }

    #endregion

    #region Logging

    [LoggerMessage(eventId: 1, level: LogLevel.Information, "The operation to {operationName} a message was aborted.")]
    private static partial void LogAbortedMessageOperation(ILogger logger, [CallerMemberName] string operationName = "");

    [LoggerMessage(eventId: 2, level: LogLevel.Error, "The operation to {operationName} a message was aborted.")]
    private static partial void LogMessageOperationError(ILogger logger, Exception ex, [CallerMemberName] string operationName = "");

    #endregion
}
