using OSK.Messages.Abstractions;
using OSK.Messages.Couriers.Pigeons.Ports;
using OSK.Messages.Messaging.Models;
using System;

namespace OSK.Messages.Couriers.Pigeons.Models;

public class CourierPigeonDescriptor : ICourierDescriptor
{
    #region Static

    public static CourierName CourierPigeons = new("Pigeons");

    #endregion

    #region ICourierDescriptor

    public CourierName Name => CourierPigeons;

    public Type CourierServiceType { get; } = typeof(IPigeonHold);

    #endregion
}
