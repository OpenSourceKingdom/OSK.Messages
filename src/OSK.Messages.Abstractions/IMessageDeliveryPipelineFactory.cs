using OSK.Hexagonal.MetaData;

namespace OSK.Messages.Abstractions;

/// <summary>
/// A factory for a given courier service integration that creates <see cref="IMessageDeliveryPipeline"/> for handling final delivery processing
/// </summary>
[HexagonalIntegration(HexagonalIntegrationType.IntegrationOptional)]
public interface IMessageDeliveryPipelineFactory
{
    /// <summary>
    /// Creates a delivery pipeline for the given recipient information
    /// </summary>
    /// <param name="details">The recipient information that the pipeline will be associated to</param>
    /// <returns>The pipeline that was created for the recipient</returns>
    IMessageDeliveryPipeline CreatePipeline(MessageRecipientDetails details);
}
