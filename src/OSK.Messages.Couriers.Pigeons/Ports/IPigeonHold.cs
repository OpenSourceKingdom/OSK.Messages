using OSK.Hexagonal.MetaData;
using OSK.Messages.Abstractions;

namespace OSK.Messages.Couriers.Pigeons.Ports;

/// <summary>
/// A courier service that utilizes pigeons as the backing mechanism to send messages locally
/// </summary>
[HexagonalIntegration(HexagonalIntegrationType.LibraryProvided)]
public interface IPigeonHold: ICourierService
{
}
