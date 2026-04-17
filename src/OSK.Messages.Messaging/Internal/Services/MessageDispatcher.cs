using OSK.Operations.Outputs.Models;
using System;
using System.Threading;
using System.Threading.Tasks;
using OSK.Messages.Abstractions;
using System.Collections.Generic;
using System.Linq;
using OSK.Operations.Outputs;
using Microsoft.Extensions.DependencyInjection;
using OSK.Messages.Messaging.Models;

namespace OSK.Messages.Messaging.Internal.Services;

internal class MessageDispatcher(IEnumerable<ICourierDescriptor> courierDescriptors, IServiceProvider serviceProvider) : IMessageDispatcher
{
    #region IEventDispatcher

    public async Task<Output> DispatchAsync(IMessage message, TimeSpan delay, DispatchOptions options, CancellationToken cancellationToken = default)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }
        if (options is null)
        {
            throw new ArgumentNullException(nameof(options));
        }
        if (delay < TimeSpan.Zero)
        {
            throw new InvalidOperationException($"Delay can not be negative");
        }

        var courierFilter = options.TargetCouriers?.ToHashSet() ?? [];
        var availableCouriers = courierFilter.Count > 0
            ? courierDescriptors.Where(descriptor => courierFilter.Contains(descriptor.Name))
            : courierDescriptors;
        if (!availableCouriers.Any())
        {
            return Out.InvalidRequest("Unable to send message because no couriers were available to deliver it.");
        }

        var scheduleDelivery = delay > TimeSpan.Zero;
        List<Output> courierOutputs = [];
        foreach (var descriptor in availableCouriers)
        {
            var courierService = (ICourierService)serviceProvider.GetRequiredService(descriptor.CourierServiceType);
            var sentOutput = scheduleDelivery
                ? await courierService.ScheduleAsync(message, delay, cancellationToken)
                : await courierService.DeliverAsync(message, cancellationToken);

            if (options.DispatchStrategy is DispatchStrategy.FirstSuccess && sentOutput.IsSuccessful)
            {
                return sentOutput;
            }

            courierOutputs.Add(sentOutput);
        }

        return courierOutputs.Count is 1
            ? courierOutputs[0]
            : Out.Multiple(courierOutputs);
    }

    #endregion
}
