using OSK.Messages.Messaging.Models;
using OSK.Messages.Messaging.Ports;
using OSK.Operations.Outputs.Models;
using OSK.Messages.Abstractions;

namespace OSK.Messages.Messaging.UnitTests._Helpers;

public class TestMiddleware(Action<MessageContext> action) : IMessageMiddleware
{
    public Task<Output> HandleAsync(MessageContext context, MessageDelegate next)
    {
        action(context);
        return next(context);
    }
}
