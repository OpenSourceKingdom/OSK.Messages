using OSK.Messages.Messaging.Models;
using System;

namespace OSK.Messages.Messaging;

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
