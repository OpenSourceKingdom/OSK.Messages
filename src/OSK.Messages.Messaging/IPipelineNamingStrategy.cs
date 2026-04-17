using OSK.Messages.Messaging.Models;

namespace OSK.Messages.Messaging;

/// <summary>
/// Represents an object that is capable of providing reproduceable, unique pipeline names given <see cref="MessageRecipientDetails"/>
/// </summary>
public interface IPipelineNamingStrategy
{
    /// <summary>
    /// Returns a unique id for the given recipient information
    /// </summary>
    /// <param name="recipientDetails">The recipient information that will be used to create a unique pipeline name</param>
    /// <returns>A unique id</returns>
   string GetPipelineName(MessageRecipientDetails recipientDetails);
}
