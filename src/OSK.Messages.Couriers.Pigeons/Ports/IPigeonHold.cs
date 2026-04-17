using OSK.Messages.Abstractions;

namespace OSK.Messages.Couriers.Pigeons.Ports;

/// <summary>
/// A courier service that utilizes pigeons as the backing mechanism to send messages locally
/// </summary>
public interface IPigeonHold: ICourierService
{
}
