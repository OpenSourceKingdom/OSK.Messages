using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using OSK.Messages.Abstractions;
using OSK.Operations.Outputs.Models;
using OSK.Messages.Messaging.Models;
using OSK.Messages.Messaging.Ports;
using OSK.Messages.Messaging.Options;

namespace OSK.Messages.Messaging.Internal.Services;

internal class MessageCenterBuilder: IMessageCenterBuilder
{
    #region Variables

    private readonly Stack<Func<IServiceProvider, IMessageMiddleware>> _middlewareFactories = [];
    private readonly Dictionary<Type, Func<IEnumerable<IMessageMiddleware>, IServiceProvider, MessageBox>> _messageBoxBuilders = [];
    private readonly MessagingOptions _options = new()
    {
        AllowInheritedRecipients = false,
        MaxParallelRecipients = 1
    };

    #endregion

    #region IMessageCenterBuilder

    public IMessageCenterBuilder WithOptions(Action<MessagingOptions> optionConfigurator)
    {
        optionConfigurator?.Invoke(_options);

        return this;
    }

    public IMessageCenterBuilder UseMiddleware(Func<MessageContext, MessageDelegate, Task<Output>> middleware)
    {
        if (middleware is null)
        {
            throw new ArgumentNullException(nameof(middleware));
        }

        _middlewareFactories.Push(_ => new MessageMiddleware(middleware));
        return this;
    }

    public IMessageCenterBuilder UseMiddleware<TMiddleware>() 
        where TMiddleware : IMessageMiddleware
    {
        _middlewareFactories.Push(sp => sp.GetRequiredService<TMiddleware>());
        return this;
    }

    public IMessageCenterBuilder AddMessageBox<TMessage>(Action<IMessageBoxConfigurator<TMessage>> configurator) 
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

        _messageBoxBuilders[boxType] = (middlewares, services) =>
        {
            var builder = new MessageBoxConfigurator<TMessage>();
            configurator(builder);

            return builder.BuildMessageBox(middlewares, services);
        };

        return this;
    }

    #endregion

    #region Api

    public IMessageCenter BuildMessageCenter(IServiceProvider services)
    {
        List<IMessageMiddleware> middlewares = [];
        foreach (var middlewareFactory in _middlewareFactories)
        {
            var middleware = middlewareFactory(services);
            middlewares.Add(middleware);
        }

        middlewares.Reverse();

        List<MessageBox> messageBoxes = [];
        foreach (var messageBoxBuilder in _messageBoxBuilders.Values)
        {
            var box = messageBoxBuilder(middlewares, services);
            messageBoxes.Add(box);
        }

        return ActivatorUtilities.CreateInstance<MessageCenter>(services, [ messageBoxes, _options ]);
    }

    #endregion
}
