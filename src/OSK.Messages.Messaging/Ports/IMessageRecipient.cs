using OSK.Hexagonal.MetaData;
using OSK.Messages.Abstractions;
using OSK.Messages.Messaging.Models;
using OSK.Operations.Outputs.Models;
using System.Threading.Tasks;

namespace OSK.Messages.Messaging.Ports;

/// <summary>
/// A recipient that is used to process a message once it is delivered
/// </summary>
/// <typeparam name="TMessage">The type of message the handler uses</typeparam>
[HexagonalIntegration(HexagonalIntegrationType.LibraryProvided, HexagonalIntegrationType.ConsumerOptional)]
public interface IMessageRecipient<TMessage>
    where TMessage: IMessage
{
    /// <summary>
    /// Processes the message
    /// </summary>
    /// <param name="context">The contextual information for the message that was delivered</param>
    /// <returns>An output for the result of the delivery</returns>
    Task<Output> HandleAsync(MessageContext<TMessage> context);
}
