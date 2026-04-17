using OSK.Hexagonal.MetaData;
using OSK.Messages.Abstractions;

namespace OSK.Messages.Couriers.EasyNetQ.Ports;

/// <summary>
/// A courier service that uses EasyNetQ as the backing integration
/// </summary>
[HexagonalIntegration(HexagonalIntegrationType.LibraryProvided)]
public interface IEasyNetQCourierService: ICourierService
{
}
