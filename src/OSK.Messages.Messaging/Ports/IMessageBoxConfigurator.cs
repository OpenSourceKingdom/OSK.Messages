using OSK.Messages.Abstractions;
using OSK.Operations.Outputs.Models;
using System;
using System.Threading.Tasks;
using OSK.Messages.Messaging.Models;
using OSK.Hexagonal.MetaData;

namespace OSK.Messages.Messaging.Ports;

/// <summary>
/// A configurator that helps to setup message boxes that receive messages
/// </summary>
/// <typeparam name="TMessage"></typeparam>
[HexagonalIntegration(HexagonalIntegrationType.LibraryProvided)]
public interface IMessageBoxConfigurator<TMessage>
    where TMessage: IMessage
{
    /// <summary>
    /// Includes middleware in the pipeline for message delivery
    /// </summary>
    /// <param name="middleware">The middleware function to include in the pipeline</param>
    /// <returns>The configurator for chaining</returns>
    IMessageBoxConfigurator<TMessage> UseMiddleware(Func<MessageContext, MessageDelegate, Task<Output>> middleware);

    /// <summary>
    /// Includes typed middleware in the pipeline for message delivery
    /// </summary>
    /// <param name="middleware">The middleware function to include in the pipeline</param>
    /// <returns>The configurator for chaining</returns>
    IMessageBoxConfigurator<TMessage> UseMiddleware(Func<MessageContext, TMessage, MessageDelegate, Task<Output>> middleware);

    /// <summary>
    /// Includes middleware that is injected via DI in the pipeline for message delivery
    /// </summary>
    /// <typeparam name="TMiddleware">The type of middleware to include in the pipeline</typeparam>
    /// <returns>The configurator for chaining</returns>
    IMessageBoxConfigurator<TMessage> UseMiddleware<TMiddleware>()
        where TMiddleware : IMessageMiddleware;

    /// <summary>
    /// Includes a recipient in the message box that will receive messages
    /// </summary>
    /// <param name="handler">The handler function for the re</param>
    /// <returns>The configurator for chaining</returns>
    IMessageBoxConfigurator<TMessage> WithRecipient(Func<MessageContext, TMessage, Task<Output>> handler);

    /// <summary>
    /// Includes a recipient in the message that will receive messages. The recipient is resolved via DI and must inherit from <see cref="MessageRecipient{TMessage}"/>
    /// </summary>
    /// <typeparam name="TRecipient">The type of the recipient to include</typeparam>
    /// <returns>The configurator for chaining</returns>
    IMessageBoxConfigurator<TMessage> WithRecipient<TRecipient>()
         where TRecipient : MessageRecipient<TMessage>;
}
