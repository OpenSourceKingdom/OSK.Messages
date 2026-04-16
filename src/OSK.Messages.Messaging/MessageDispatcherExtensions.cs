using OSK.Operations.Outputs.Models;
using System;
using System.Threading;
using System.Threading.Tasks;
using OSK.Messages.Abstractions;

namespace OSK.Messages.Messaging;

public static class MessageDispatcherExtensions
{
    public static Task<Output> DispatchAsync(this IMessageDispatcher dispatcher, IMessage message, CancellationToken cancellationToken = default)
        => dispatcher.DispatchAsync(message, TimeSpan.Zero, DispatchOptions.Default, cancellationToken);
}
