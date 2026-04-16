using OSK.Messages.Messaging.Models;

namespace OSK.Messages.Messaging;

public interface IPipelineNamingStrategy
{
   string GetPipelineName(MessageRecipientDetails recipientDetails);
}
