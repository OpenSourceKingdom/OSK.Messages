using System.Threading.Tasks;
using OSK.Operations.Outputs.Models;
using OSK.Messages.Messaging.Models;

namespace OSK.Messages.Messaging.Ports;

public interface IMessageMiddleware
{
    Task<Output> HandleAsync(MessageContext context, MessageDelegate next);
}
