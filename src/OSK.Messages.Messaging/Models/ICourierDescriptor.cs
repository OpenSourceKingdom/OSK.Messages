using System;
using OSK.Messages.Abstractions;

namespace OSK.Messages.Messaging.Models;

public interface ICourierDescriptor
{
    public CourierName Name { get; }

    public Type CourierServiceType { get; }
}
