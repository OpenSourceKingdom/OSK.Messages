using OSK.Messages.Abstractions;
using OSK.Operations.Outputs.Models;
using System;
using System.Threading.Tasks;
using OSK.Messages.Messaging.Models;

namespace OSK.Messages.Messaging.Ports;

/// <summary>
/// A configurator that helps to setup message boxes that receive messages
/// </summary>
/// <typeparam name="TMessage"></typeparam>
public interface IMessageBoxConfigurator<TMessage>
    where TMessage: IMessage
{
    /// <summary>
    /// Includes middleware in the pipeline for message delivery
    /// </summary>
    /// <param name="middleware">The middleware function to include in the pipeline</param>
    /// <returns>The configurator for chaining</returns>
    IMessageBoxConfigurator<TMessage> UseMiddleware(Func<MessageContext<TMessage>, MessageDelegate<TMessage>, Task<Output>> middleware);

    /// <summary>
    /// Includes middleware that is injected via DI in the pipeline for message delivery
    /// </summary>
    /// <typeparam name="TMiddleware">The type of middleware to include in the pipeline</typeparam>
    /// <returns>The configurator for chaining</returns>
    IMessageBoxConfigurator<TMessage> UseMiddleware<TMiddleware>()
        where TMiddleware : IMessageMiddleware<TMessage>;

    /// <summary>
    /// Includes a recipient using a function into the messsage delivery
    /// </summary>
    /// <param name="handler">The recipient function to include in the messsage delivery</param>
    /// <returns>The configurator for chaining</returns>
    IMessageBoxConfigurator<TMessage> WithRecipient(Func<MessageContext<TMessage>, Task<Output>> handler);

    /// <summary>
    /// Includes a recipient using a type injected via DI in the pipeline for messaage delivery
    /// </summary>
    /// <typeparam name="THandler">The recipient type to include in message delivery</typeparam>
    /// <returns>Thje configurator for chaining</returns>
    IMessageBoxConfigurator<TMessage> WithRecipient<THandler>()
        where THandler : IMessageHandler<TMessage>;
}
