using OSK.Messages.Abstractions;
using OSK.Operations.Outputs;
using OSK.Operations.Outputs.Models;

namespace OSK.Messages.Messaging.UnitTests._Helpers;

public class TestRecipient(Action<MessageContext> action) : IMessageRecipient
{
    public Type MessageType => typeof(object);

    public Task<Output> ReceiveAsync(MessageContext context)
    {
        action(context);
        return Task.FromResult(Out.Success());
    }
}
