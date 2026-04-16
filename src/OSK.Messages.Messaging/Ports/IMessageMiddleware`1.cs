using System.Threading.Tasks;
using OSK.Messages.Abstractions;
using OSK.Operations.Outputs.Models;
using OSK.Messages.Messaging.Models;

namespace OSK.Messages.Messaging.Ports;

public interface IMessageMiddleware<TMessage>
    where TMessage : IMessage
{
    Task<Output> HandleAsync(MessageContext<TMessage> context, MessageDelegate<TMessage> next);
}
