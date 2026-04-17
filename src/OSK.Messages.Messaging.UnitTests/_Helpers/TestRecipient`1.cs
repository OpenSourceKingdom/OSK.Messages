using OSK.Messages.Abstractions;
using OSK.Messages.Messaging.Models;
using OSK.Messages.Messaging.Ports;
using OSK.Operations.Outputs;
using OSK.Operations.Outputs.Models;

namespace OSK.Messages.Messaging.UnitTests._Helpers;

public class TestRecipient<TMessage>(Action<TMessage> action) : IMessageRecipient<TMessage>
    where TMessage : IMessage
{
    public Task<Output> HandleAsync(MessageContext<TMessage> context)
    {
        action(context.Message);
        return Task.FromResult(Out.Success());
    }
}
