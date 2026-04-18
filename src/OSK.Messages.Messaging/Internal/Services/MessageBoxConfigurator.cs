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

internal class MessageBoxConfigurator<TMessage> : MessageBoxBuilder, IMessageBoxConfigurator<TMessage>
    where TMessage : IMessage
{
    #region Variables

    private readonly Stack<Func<IServiceProvider, IMessageMiddleware<TMessage>>> _middlewareFactories = [];
    private List<Func<IServiceProvider, IMessageRecipient<TMessage>>> _recipientFactories = [];

    #endregion

    #region IMessageBoxConfigurator

    public IMessageBoxConfigurator<TMessage> WithRecipient(Func<MessageContext<TMessage>, Task<Output>> handler)
    {
        if (handler is null)
        {
            throw new ArgumentNullException(nameof(handler));
        }

        _recipientFactories.Add(_ => new MessageRecipient<TMessage>(handler));
        return this;
    }

    public IMessageBoxConfigurator<TMessage> WithRecipient<THandler>() 
        where THandler : IMessageRecipient<TMessage>
    {
        _recipientFactories.Add(sp => sp.GetRequiredService<THandler>());
        return this;
    }

    public IMessageBoxConfigurator<TMessage> UseMiddleware(Func<MessageContext<TMessage>, MessageDelegate<TMessage>, Task<Output>> middleware)
    {
        if (middleware is null)
        {
            throw new ArgumentNullException();
        }

        _middlewareFactories.Push(_ => new MessageMiddleware<TMessage>(middleware));
        return this;
    }

    public IMessageBoxConfigurator<TMessage> UseMiddleware<TMiddleware>() 
        where TMiddleware : IMessageMiddleware<TMessage>
    {
        _middlewareFactories.Push(sp => sp.GetRequiredService<TMiddleware>());
        return this;
    }

    #endregion

    #region MessageBoxBuilder Overrides

    public override MessageBox BuildMessageBox(IEnumerable<IMessageMiddleware> globalMiddlewares, IServiceProvider services)
    {
        if (services is null)
        {
            throw new ArgumentNullException(nameof(services));
        }
        if (_recipientFactories.Count is 0)
        {
            throw new InvalidOperationException($"Unable to build message box for messages of type {typeof(TMessage)} because no message handler was configured to process it.");
        }

        return new MessageBox<TMessage>([.. _recipientFactories.Select(factory =>
        {
            var recipient = factory(services);
            return new MessageBoxRecipient<TMessage>(recipient, CreateDelegate(recipient, globalMiddlewares, services));
        })]);
    }

    #endregion

    #region Helpers

    private MessageDelegate CreateDelegate(IMessageRecipient<TMessage> recipient, IEnumerable<IMessageMiddleware> globalMiddlewares, IServiceProvider services)
    {
        MessageDelegate<TMessage> typedPipeline = recipient.HandleAsync;
        while (_middlewareFactories.Count > 0)
        {
            var middlewareFactory = _middlewareFactories.Pop();
            var nextMiddleware = middlewareFactory(services);
            var next = typedPipeline;
            typedPipeline = context => nextMiddleware.HandleAsync(context, next);
        }

        MessageDelegate messenger = context =>
        {
            return context is MessageContext<TMessage> messageContext
                ? typedPipeline(messageContext)
                : Task.FromResult(Out.Success());
        };

        foreach (var middleware in globalMiddlewares)
        {
            var next = messenger;
            messenger = context => middleware.HandleAsync(context, next);
        }

        return messenger;
    }

    #endregion
}
