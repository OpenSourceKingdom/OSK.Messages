using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using OSK.Messages.Abstractions;
using OSK.Operations.Outputs.Models;
using OSK.Messages.Messaging.Models;
using OSK.Messages.Messaging.Ports;
using OSK.Messages.Messaging.Options;
using System.Linq;

namespace OSK.Messages.Messaging.Internal.Services;

internal class MessageCenterBuilder: IMessageCenterBuilder
{
    #region Variables

    private readonly Stack<Func<IServiceProvider, IMessageMiddleware>> _middlewareFactories = [];
    private readonly Dictionary<Type, Func<IEnumerable<IMessageRecipient>, List<IMessageMiddleware>, IServiceProvider, MessageBox>> _messageBoxBuilders = [];
    private readonly MessagingOptions _options = new()
    {
        AllowInheritedMessageDelivery = false
    };

    #endregion

    #region IMessageCenterBuilder

    public IMessageCenterBuilder WithOptions(Action<MessagingOptions> optionConfigurator)
    {
        if (optionConfigurator is null)
        {
            throw new ArgumentNullException(nameof(optionConfigurator));
        }

        optionConfigurator.Invoke(_options);

        return this;
    }

    public IMessageCenterBuilder UseMiddleware(Func<MessageContext, MessageDelegate, Task<Output>> middleware)
    {
        if (middleware is null)
        {
            throw new ArgumentNullException(nameof(middleware));
        }

        _middlewareFactories.Push(_ => new MessageFunctionMiddleware(middleware));
        return this;
    }

    public IMessageCenterBuilder UseMiddleware<TMiddleware>() 
        where TMiddleware : IMessageMiddleware
    {
        _middlewareFactories.Push(sp => sp.GetRequiredService<TMiddleware>());
        return this;
    }

    public IMessageCenterBuilder ConfigureMessageBox<TMessage>(Action<IMessageBoxConfigurator<TMessage>> configurator) 
        where TMessage : IMessage
    {
        if (configurator is null)
        {
            throw new ArgumentNullException(nameof(configurator));
        }
        
        var boxType = typeof(TMessage);
        if (_messageBoxBuilders.TryGetValue(boxType, out _))
        { 
            return this;
        }

        _messageBoxBuilders[boxType] = (registeredRecipients, middlewares, services) =>
        {
            var builder = new MessageBoxConfigurator<TMessage>();
            configurator(builder);

            return builder.ConfigureMessageBox(registeredRecipients, middlewares, services);
        };

        return this;
    }

    #endregion

    #region Api

    public IMessageCenter BuildMessageCenter(IServiceProvider services)
    {
        if (services is null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        List<IMessageMiddleware> middlewares = [];
        foreach (var middlewareFactory in _middlewareFactories)
        {
            var middleware = middlewareFactory(services);
            middlewares.Add(middleware);
        }

        var registeredMessageRecipients = services.GetService<IEnumerable<IMessageRecipient>>()?
                                                  .GroupBy(recipient => recipient.MessageType)
                                                  .ToDictionary(recipientGroup => recipientGroup.Key, recipientGroup => recipientGroup.Select(recipient => recipient)) ?? [];

        foreach (var registeredMessageRecipientKvp in registeredMessageRecipients)
        {
            if (!_messageBoxBuilders.TryGetValue(registeredMessageRecipientKvp.Key, out _))
            {
                _messageBoxBuilders[registeredMessageRecipientKvp.Key] = (registeredRecipients, middlewares, services) =>
                {
                    return new MessageBox(registeredMessageRecipientKvp.Key, [.. registeredMessageRecipientKvp.Value.Select(recipient =>
                    {
                        MessageDelegate messenger = recipient.ReceiveAsync;
                        return new MessageBoxRecipient(recipient, messenger);
                    })]);
                };
            }
        }

        List<MessageBox> messageBoxes = [];
        foreach (var messageBoxTypBuilderKvp in _messageBoxBuilders)
        {
            var registeredRecipients = registeredMessageRecipients.TryGetValue(messageBoxTypBuilderKvp.Key, out var recipients) ? recipients : Enumerable.Empty<IMessageRecipient>();
            var box = messageBoxTypBuilderKvp.Value(registeredRecipients, middlewares, services);
            messageBoxes.Add(box);
        }

        return ActivatorUtilities.CreateInstance<MessageCenter>(services, [ messageBoxes, _options ]);
    }

    #endregion
}
