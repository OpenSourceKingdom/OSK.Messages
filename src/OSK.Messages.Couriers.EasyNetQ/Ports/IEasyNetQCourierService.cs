using OSK.Messages.Abstractions;

namespace OSK.Messages.Couriers.EasyNetQ.Ports;

/// <summary>
/// A courier service that uses EasyNetQ as the backing integration
/// </summary>
public interface IEasyNetQCourierService: ICourierService
{
}
