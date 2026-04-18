using OSK.Operations.Outputs.Models;
using System.Threading.Tasks;
using OSK.Messages.Abstractions;

namespace OSK.Messages.Messaging.Models;

public delegate Task<Output> MessageDelegate(MessageContext context);
public delegate Task<Output> MessageDelegate<TMessage>(MessageContext<TMessage> context)
    where TMessage: IMessage;
