using OSK.Hexagonal.MetaData;
using OSK.Messages.Abstractions;
using OSK.Messages.Messaging.Models;
using OSK.Messages.Messaging.Options;
using OSK.Operations.Outputs.Models;
using System;
using System.Threading.Tasks;

namespace OSK.Messages.Messaging.Ports;

/// <summary>
/// A builder factory that helps to build custom tailored and configured Message centers
/// </summary>
[HexagonalIntegration(HexagonalIntegrationType.LibraryProvided)]
public interface IMessageCenterBuilder
{
    /// <summary>
    /// Sets the messaging options a message center will use to process messages
    /// </summary>
    /// <param name="optionConfigurator">The action configurator to apply to the options</param>
    /// <returns>The builder for chaining</returns>
    IMessageCenterBuilder WithOptions(Action<MessagingOptions> optionConfigurator);

    /// <summary>
    /// Adds middleware at a global level that will be applied to all message deliveries in the message center
    /// </summary>
    /// <param name="middleware">The middleware function to include in the message delivery</param>
    /// <returns>The builder for chaining</returns>
    IMessageCenterBuilder UseMiddleware(Func<MessageContext, MessageDelegate, Task<Output>> middleware);

    /// <summary>
    /// Adds middleware at a global level that will be applied to all message deliveries in the message center
    /// </summary>
    /// <typeparam name="TMiddleware">The type of middleware injected via DI</typeparam>
    /// <returns>The builder for chaining</returns>
    IMessageCenterBuilder UseMiddleware<TMiddleware>()
        where TMiddleware : IMessageMiddleware;

    /// <summary>
    /// Adds a message box to receive messages of type <typeparamref name="TMessage"/>
    /// </summary>
    /// <typeparam name="TMessage">The type of message the box will receive</typeparam>
    /// <param name="configurator">The action configurator to apply to the message box</param>
    /// <returns>The builder for chaining</returns>
    IMessageCenterBuilder AddMessageBox<TMessage>(Action<IMessageBoxConfigurator<TMessage>> configurator)
        where TMessage: IMessage;
}
