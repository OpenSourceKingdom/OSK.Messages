using OSK.Hexagonal.MetaData;
using OSK.Messages.Messaging.Models;
using System.Threading.Tasks;
using OSK.Messages.Abstractions;

namespace OSK.Messages.Messaging.Ports;

/// <summary>
/// A generic middleware that can be applied to any message delivery
/// </summary>
[HexagonalIntegration(HexagonalIntegrationType.LibraryProvided, HexagonalIntegrationType.ConsumerOptional)]
public interface IMessageMiddleware
{
    /// <summary>
    /// Runs the middleware during message delivery
    /// </summary>
    /// <param name="context">The context for the message being delivered</param>
    /// <param name="next">The next middleware or handler to call in the pipeline of message delivery</param>
    Task HandleAsync(MessageContext context, MessageDelegate next);
}
