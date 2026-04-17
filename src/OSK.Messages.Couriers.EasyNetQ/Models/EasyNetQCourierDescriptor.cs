using OSK.Messages.Abstractions;
using System;
using OSK.Messages.Messaging.Models;
using OSK.Messages.Couriers.EasyNetQ.Ports;

namespace OSK.Messages.Couriers.EasyNetQ.Models;

public class EasyNetQCourierDescriptor : ICourierDescriptor
{
    #region Static

    public static CourierName EasyNetQ = new("EasyNetQ");

    #endregion

    #region ICourierDescriptor

    public CourierName Name => EasyNetQ;

    public Type CourierServiceType { get; } = typeof(IEasyNetQCourierService);

    #endregion
}
