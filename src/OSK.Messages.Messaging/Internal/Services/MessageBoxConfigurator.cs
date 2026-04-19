using Microsoft.Extensions.DependencyInjection;
using OSK.Messages.Abstractions;
using OSK.Operations.Outputs;
using OSK.Operations.Outputs.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OSK.Messages.Messaging.Models;
using OSK.Messages.Messaging.Ports;

namespace OSK.Messages.Messaging.Internal.Services;

internal class MessageBoxConfigurator<TMessage> : IMessageBoxConfigurator<TMessage>
    where TMessage : IMessage
{
    #region Variables

    private readonly List<Func<IServiceProvider, IMessageMiddleware>> _middlewareFactories = [];
    private List<Func<IServiceProvider, IMessageRecipient>> _recipientFactories = [];

    #endregion

    #region IMessageBoxConfigurator

    public IMessageBoxConfigurator<TMessage> UseMiddleware(Func<MessageContext, MessageDelegate, Task<Output>> middleware)
    {
        if (middleware is null)
        {
            throw new ArgumentNullException();
        }

        _middlewareFactories.Add(_ => new MessageFunctionMiddleware(middleware));
        return this;
    }

    public IMessageBoxConfigurator<TMessage> UseMiddleware(Func<MessageContext, TMessage, MessageDelegate, Task<Output>> middleware)
    {
        if (middleware is null)
        {
            throw new ArgumentNullException();
        }

        _middlewareFactories.Add(_ => new MessageFunctionMiddleware<TMessage>(middleware));
        return this;
    }

    public IMessageBoxConfigurator<TMessage> UseMiddleware<TMiddleware>() 
        where TMiddleware : IMessageMiddleware
    {
        _middlewareFactories.Add(sp => sp.GetRequiredService<TMiddleware>());
        return this;
    }

    public IMessageBoxConfigurator<TMessage> WithRecipient(Func<MessageContext, TMessage, Task<Output>> handler)
    {
        if (handler is null)
        {
            throw new ArgumentNullException(nameof(handler));
        }

        _recipientFactories.Add(_ => new MessageFunctionRecipient<TMessage>(handler));
        return this;
    }

    public IMessageBoxConfigurator<TMessage> WithRecipient<THandler>()
        where THandler : MessageRecipient<TMessage>
    {
        _recipientFactories.Add(sp => sp.GetRequiredService<THandler>());
        return this;
    }

    #endregion

    #region Api

    public MessageBox ConfigureMessageBox(IEnumerable<IMessageRecipient> registeredRecipients, List<IMessageMiddleware> globalMiddlewares, IServiceProvider services)
    {
        if (services is null)
        {
            throw new ArgumentNullException(nameof(services));
        }
        if (_recipientFactories.Count is 0)
        {
            throw new InvalidOperationException($"Unable to build message box for messages of type {typeof(TMessage)} because no message handler was configured to process it.");
        }

        return new MessageBox(typeof(TMessage), [.. _recipientFactories.Select(factory =>
        {
            var recipient = factory(services);
            return new MessageBoxRecipient(recipient, CreateDelegate(recipient, globalMiddlewares, services));
        })]);
    }

    #endregion

    #region Helpers

    private MessageDelegate CreateDelegate(IMessageRecipient recipient, List<IMessageMiddleware> globalMiddlewares, IServiceProvider services)
    {
        MessageDelegate messenger = recipient.ReceiveAsync;

        for (var i = _middlewareFactories.Count - 1; i >= 0; i--)
        {
            var middleware = _middlewareFactories[i](services);
            var next = messenger;
            messenger = context => middleware.HandleAsync(context, next);
        }

        for (var i = globalMiddlewares.Count - 1; i >= 0; i--)
        {
            var middleware = globalMiddlewares[i];
            var next = messenger;
            messenger = context => middleware.HandleAsync(context, next);
        }

        return messenger;
    }

    #endregion
}
