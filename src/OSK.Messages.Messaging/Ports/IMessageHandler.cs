using OSK.Messages.Abstractions;
using OSK.Operations.Outputs.Models;
using System.Threading.Tasks;
using OSK.Messages.Messaging.Models;

namespace OSK.Messages.Messaging.Ports;

public interface IMessageHandler<TMessage>
    where TMessage: IMessage
{
    Task<Output> HandleAsync(MessageContext<TMessage> context);
}
