using OSK.Messages.Abstractions;
using OSK.Operations.Outputs.Models;
using System;
using System.Threading.Tasks;
using OSK.Messages.Messaging.Models;

namespace OSK.Messages.Messaging.Ports;

public interface IMessageBoxConfigurator<TMessage>
    where TMessage: IMessage
{
    IMessageBoxConfigurator<TMessage> UseMiddleware(Func<MessageContext<TMessage>, MessageDelegate<TMessage>, Task<Output>> middleware);

    IMessageBoxConfigurator<TMessage> UseMiddleware<TMiddleware>()
        where TMiddleware : IMessageMiddleware<TMessage>;

    IMessageBoxConfigurator<TMessage> WithRecipient(Func<MessageContext<TMessage>, Task<Output>> handler);

    IMessageBoxConfigurator<TMessage> WithRecipient<THandler>()
        where THandler : IMessageHandler<TMessage>;
}
