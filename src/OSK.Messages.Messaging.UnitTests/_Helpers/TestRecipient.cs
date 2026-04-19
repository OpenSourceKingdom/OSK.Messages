using OSK.Messages.Abstractions;

namespace OSK.Messages.Messaging.UnitTests._Helpers;

public class TestRecipient(Action<MessageContext> action) : IMessageRecipient
{
    public Type MessageType => typeof(object);

    public Task ReceiveAsync(MessageContext context)
    {
        action(context);
        return Task.CompletedTask;
    }
}
