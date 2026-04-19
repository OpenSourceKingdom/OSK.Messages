using OSK.Messages.Abstractions;
using OSK.Operations.Outputs;
using OSK.Operations.Outputs.Models;

namespace OSK.Messages.Messaging.UnitTests._Helpers;

public class TestRecipient<TMessage>(Action<TMessage> action) : MessageRecipient<TMessage>
    where TMessage : IMessage
{
    protected override Task<Output> ReceiveAsync(MessageContext context, TMessage message)
    {
        action(message);
        return Task.FromResult(Out.Success());
    }
}
