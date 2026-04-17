using OSK.Hexagonal.MetaData;
using OSK.Messages.Abstractions;
using OSK.Messages.Messaging.Models;
using OSK.Operations.Outputs.Models;
using System.Threading.Tasks;

namespace OSK.Messages.Messaging.Ports;

/// <summary>
/// A middleware that runs on a specific message during delivery
/// </summary>
/// <typeparam name="TMessage">The message type the middleware will be applied to</typeparam>
[HexagonalIntegration(HexagonalIntegrationType.LibraryProvided, HexagonalIntegrationType.ConsumerOptional)]
public interface IMessageMiddleware<TMessage>
    where TMessage : IMessage
{
    /// <summary>
    /// Runs the middleware during message delivery
    /// </summary>
    /// <param name="context">The context for the message being delivered</param>
    /// <param name="next">The next middleware or handler to call in the pipeline of message delivery</param>
    /// <returns>An output for the result of the middleware</returns>
    Task<Output> HandleAsync(MessageContext<TMessage> context, MessageDelegate<TMessage> next);
}
