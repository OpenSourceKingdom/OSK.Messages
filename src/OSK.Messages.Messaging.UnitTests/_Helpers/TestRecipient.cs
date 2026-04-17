using OSK.Messages.Abstractions;
using OSK.Messages.Messaging.Models;
using OSK.Messages.Messaging.Ports;
using OSK.Operations.Outputs;
using OSK.Operations.Outputs.Models;

namespace OSK.Messages.Messaging.UnitTests._Helpers;

public class TestRecipient(Action<MessageContext<IMessage>> action) : IMessageRecipient<IMessage>
{
    public Task<Output> HandleAsync(MessageContext<IMessage> context)
    {
        action(context);
        return Task.FromResult(Out.Success());
    }
}
