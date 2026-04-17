using OSK.Messages.Messaging.Models;
using System;

namespace OSK.Messages.Messaging;

/// <summary>
/// A naming strategy that utilizes the message and recipient types to create a unique pipeline id
/// </summary>
public class PipelineTypeNamingStrategy : IPipelineNamingStrategy
{
    #region PipelineNamingStrategy

    public string GetPipelineName(MessageRecipientDetails recipientDetails)
    {
        if (recipientDetails is null)
        {
            throw new ArgumentNullException(nameof(recipientDetails));
        }

        return $"{recipientDetails.RecipientType.FullName}.{recipientDetails.MessageType.FullName}";
    }

    #endregion
}
