using OSK.Operations.Outputs.Models;
using System;
using System.Threading.Tasks;
using OSK.Messages.Messaging.Models;
using OSK.Messages.Messaging.Ports;

namespace OSK.Messages.Messaging.Internal.Services;

internal class MessageMiddleware(Func<MessageContext, MessageDelegate, Task<Output>> middleware) : IMessageMiddleware
{
    public Task<Output> HandleAsync(MessageContext context, MessageDelegate next)
        => middleware(context, next);
}
