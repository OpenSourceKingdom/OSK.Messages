using OSK.Messages.Abstractions;
using System;
using System.Threading.Tasks;
using OSK.Operations.Outputs.Models;
using OSK.Messages.Messaging.Models;
using OSK.Messages.Messaging.Options;

namespace OSK.Messages.Messaging.Ports;

public interface IMessageCenterBuilder
{
    IMessageCenterBuilder WithOptions(Action<MessagingOptions> optionConfigurator);

    IMessageCenterBuilder UseMiddleware(Func<MessageContext, MessageDelegate, Task<Output>> middleware);

    IMessageCenterBuilder UseMiddleware<TMiddleware>()
        where TMiddleware : IMessageMiddleware;

    IMessageCenterBuilder AddMessageBox<TMessage>(Action<IMessageBoxConfigurator<TMessage>> configurator)
        where TMessage: IMessage;
}
