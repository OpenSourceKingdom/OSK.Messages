using System;
using OSK.Messages.Abstractions;

namespace OSK.Messages.Messaging.Models;

/// <summary>
/// Describes the various parts of a courier so that it can be utilized when messages are dispatched
/// </summary>
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
