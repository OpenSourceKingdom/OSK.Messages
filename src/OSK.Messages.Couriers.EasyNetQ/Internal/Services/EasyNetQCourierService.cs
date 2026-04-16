using EasyNetQ;
using OSK.Messages.Abstractions;
using OSK.Operations.Outputs;
using OSK.Operations.Outputs.Models;
using System;
using System.Threading;
using System.Threading.Tasks;
using IMessage = OSK.Messages.Abstractions.IMessage;

namespace OSK.Messages.Couriers.EasyNetQ.Internal.Services;

internal class EasyNetQCourierService(IBus bus) : ICourierService
{
    #region ICourierService

    public async Task<Output> DeliverAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default)
        where TMessage : IMessage
    {
        if (message is null) 
        {
            throw new ArgumentNullException(nameof(message));
        }

        try
        {
            await bus.PubSub.PublishAsync(message, cancellationToken).ConfigureAwait(false);
            return Out.Success();
        }
        catch (Exception ex)
        {
            return Out.Fault(ex);
        }
    }

    public async Task<Output> ScheduleAsync<TMessage>(TMessage message, TimeSpan delay, CancellationToken cancellationToken = default)
        where TMessage : IMessage
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }
        if (delay <= TimeSpan.Zero)
        {
            return await DeliverAsync(message, cancellationToken);
        }

        try
        {
            await bus.Scheduler.FuturePublishAsync(message, delay, cancellationToken).ConfigureAwait(false);
            return Out.Success();
        }
        catch (Exception ex)
        {
            return Out.Fault(ex);
        }
    }

    #endregion
}