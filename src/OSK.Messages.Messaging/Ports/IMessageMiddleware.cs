using System.Threading.Tasks;
using OSK.Operations.Outputs.Models;
using OSK.Messages.Messaging.Models;

namespace OSK.Messages.Messaging.Ports;

/// <summary>
/// A generic middleware that can be applied to any message delivery
/// </summary>
public interface IMessageMiddleware
{
    /// <summary>
    /// Runs the middleware during message delivery
    /// </summary>
    /// <param name="context">The context for the message being delivered</param>
    /// <param name="next">The next middleware or handler to call in the pipeline of message delivery</param>
    /// <returns>An output for the result of the middleware</returns>
    Task<Output> HandleAsync(MessageContext context, MessageDelegate next);
}
