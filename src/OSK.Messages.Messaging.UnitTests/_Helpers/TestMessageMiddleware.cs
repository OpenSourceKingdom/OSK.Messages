using OSK.Messages.Abstractions;
using OSK.Messages.Messaging.Models;
using OSK.Messages.Messaging.Ports;
using OSK.Operations.Outputs.Models;

namespace OSK.Messages.Messaging.UnitTests._Helpers;

public class TestMessageMiddleware(Action<MessageContext<IMessage>> action) : IMessageMiddleware<IMessage>
{
    public Task<Output> HandleAsync(MessageContext<IMessage> context, MessageDelegate<IMessage> next)
    {
        action(context);
        return next(context);
    }
}
