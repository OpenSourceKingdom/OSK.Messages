using OSK.Messages.Abstractions;
using OSK.Messages.Messaging.Models;

namespace OSK.Messages.Messaging.UnitTests._Helpers;

public class TestMessageMiddleware(Action<IMessage> action) : MessageMiddleware<IMessage>
{
    protected override Task HandleAsync(MessageContext context, IMessage message, MessageDelegate next)
    {
        action(message);
        return next(context);
    }
}
