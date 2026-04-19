using System;
using System.Threading.Tasks;
using OSK.Messages.Messaging.Models;
using OSK.Messages.Messaging.Ports;
using OSK.Messages.Abstractions;

namespace OSK.Messages.Messaging.Internal.Services;

internal class MessageFunctionMiddleware(Func<MessageContext, MessageDelegate, Task> middleware) : IMessageMiddleware
{
    public Task HandleAsync(MessageContext context, MessageDelegate next)
        => middleware(context, next);
}
