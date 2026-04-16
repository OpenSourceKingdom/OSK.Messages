using OSK.Messages.Messaging.Models;

namespace OSK.Messages.Messaging.Ports;

public interface IMessageDeliveryPipelineFactory
{
    IMessageDeliveryPipeline CreatePipeline(MessageRecipientDetails details);
}
