using System;
using OSK.Hexagonal.MetaData;

namespace OSK.Messages.Abstractions;

/// <summary>
/// Describes the various parts of a courier so that it can be utilized when messages are dispatched
/// </summary>
[HexagonalIntegration(HexagonalIntegrationType.IntegrationRequired)]
public interface ICourierDescriptor
{
    /// <summary>
    /// The unique courier name
    /// </summary>
    public CourierName Name { get; }

    /// <summary>
    /// The service type that is used to dispatch messages
    /// </summary>
    public Type CourierServiceType { get; }
}
