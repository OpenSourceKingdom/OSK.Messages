using Moq;
using OSK.Messages.Abstractions;
using OSK.Messages.Messaging.Internal.Services;
using OSK.Messages.Messaging.Models;
using OSK.Messages.Messaging.UnitTests._Helpers;
using OSK.Operations.Outputs;
using OSK.Operations.Outputs.Models;

namespace OSK.Messages.Messaging.UnitTests.Internal.Services;

public class MessageDispatcherTests
{
    #region Variables

    private readonly Mock<IServiceProvider> _mockProvider;
    private readonly List<ICourierDescriptor> _descriptors;

    private readonly MessageDispatcher _dispatcher;

    #endregion

    #region Constructors

    public MessageDispatcherTests()
    {
        _descriptors = [];
        _mockProvider = new();

        _dispatcher = new(_descriptors, _mockProvider.Object);
    }

    #endregion

    #region DispatchAsync

    [Fact]
    public async Task DispatchAsync_NullMessage_ThrowsArgumentNullException()
    {
        // Arrange/Act/Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _dispatcher.DispatchAsync(null!, TimeSpan.Zero, new DispatchOptions()));
    }

    [Fact]
    public async Task DispatchAsync_NullDispatchOptions_ThrowsArgumentNullException()
    {
        // Arrange/Act/Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _dispatcher.DispatchAsync(new TestChildMessage(), TimeSpan.Zero, null!));
    }

    [Fact]
    public async Task DispatchAsync_NegativeDelay_ThrowsInvalidOperationException()
    {
        // Arrange/Act/Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _dispatcher.DispatchAsync(new TestChildMessage(), TimeSpan.FromSeconds(-1), new DispatchOptions()));
    }

    [Fact]
    public async Task DispatchAsync_NoCouriers_ReturnsInvalidRequest()
    {
        // Arrange/Act
        var output = await _dispatcher.DispatchAsync(new TestChildMessage(), TimeSpan.Zero, new DispatchOptions());

        // Assert
        Assert.False(output.IsSuccessful);
        Assert.Equal(OutputStatus.InvalidRequest, output.StatusCode.Status);
    }

    [Fact]
    public async Task DispatchAsync_SingleCourier_FilterRemoves_ReturnsInvalidRequest()
    {
        // Arrange
        var descriptor = new Mock<ICourierDescriptor>();
        descriptor.SetupGet(m => m.Name)
            .Returns(new CourierName("Hello"));

        _descriptors.Add(descriptor.Object);

        // Act
        var output = await _dispatcher.DispatchAsync(new TestChildMessage(), TimeSpan.Zero, new DispatchOptions()
        {
            TargetCouriers = [ new CourierName("Wedgy") ]
        });

        // Assert
        Assert.False(output.IsSuccessful);
        Assert.Equal(OutputStatus.InvalidRequest, output.StatusCode.Status);
    }


    [Fact]
    public async Task DispatchAsync_SingleCourier_FiltersToCourier_ReturnsSuccessfully()
    {
        // Arrange
        var descriptor = new Mock<ICourierDescriptor>();
        descriptor.SetupGet(m => m.Name)
            .Returns(new CourierName("Hello"));
        descriptor.SetupGet(m => m.CourierServiceType)
            .Returns(typeof(IMessage));

        _descriptors.Add(descriptor.Object);

        var mockCourierService = new Mock<ICourierService>();
        mockCourierService.Setup(m => m.DeliverAsync(It.IsAny<IMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Out.Success());

        _mockProvider.Setup(m => m.GetService(It.Is<Type>(t => t == typeof(IMessage))))
            .Returns(mockCourierService.Object);


        // Act
        var output = await _dispatcher.DispatchAsync(new TestChildMessage(), TimeSpan.Zero, new DispatchOptions()
        {
            TargetCouriers = [new CourierName("Hello")]
        });

        // Assert
        Assert.True(output.IsSuccessful);

        mockCourierService.Verify(m => m.ScheduleAsync(It.IsAny<IMessage>(), It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task DispatchAsync_MultipleCouriers_NoFilter_FirstSuccess_FirstReturnsSuccessNeverHitsSecond_ReturnsSuccessfully()
    {
        // Arrange
        var descriptor = new Mock<ICourierDescriptor>();
        descriptor.SetupGet(m => m.Name)
            .Returns(new CourierName("Hello"));
        descriptor.SetupGet(m => m.CourierServiceType)
            .Returns(typeof(IMessage));

        var descriptor2 = new Mock<ICourierDescriptor>();
        descriptor2.SetupGet(m => m.Name)
            .Returns(new CourierName("Hello2"));
        descriptor2.SetupGet(m => m.CourierServiceType)
            .Returns(typeof(int));

        _descriptors.Add(descriptor.Object);
        _descriptors.Add(descriptor2.Object);

        var mockCourierService = new Mock<ICourierService>();
        mockCourierService.Setup(m => m.DeliverAsync(It.IsAny<IMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Out.Success());

        var mockCourierService2 = new Mock<ICourierService>();
        mockCourierService2.Setup(m => m.DeliverAsync(It.IsAny<IMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Out.Success());

        _mockProvider.Setup(m => m.GetService(It.Is<Type>(t => t == typeof(IMessage))))
            .Returns(mockCourierService.Object);

        _mockProvider.Setup(m => m.GetService(It.Is<Type>(t => t == typeof(int))))
            .Returns(mockCourierService2.Object);

        // Act
        var output = await _dispatcher.DispatchAsync(new TestChildMessage(), TimeSpan.Zero, new DispatchOptions()
        {
            DispatchStrategy = DispatchStrategy.FirstSuccess
        });

        // Assert
        Assert.True(output.IsSuccessful);

        mockCourierService.Verify(m => m.ScheduleAsync(It.IsAny<IMessage>(), It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()), Times.Never);

        mockCourierService2.Verify(m => m.DeliverAsync(It.IsAny<IMessage>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task DispatchAsync_MultipleCouriers_NoFilter_FirstSuccess_TimeSpanGreaterThanZero_SchedulesDelivery_FirstReturnsSuccessNeverHitsSecond_ReturnsSuccessfully()
    {
        // Arrange
        var descriptor = new Mock<ICourierDescriptor>();
        descriptor.SetupGet(m => m.Name)
            .Returns(new CourierName("Hello"));
        descriptor.SetupGet(m => m.CourierServiceType)
            .Returns(typeof(IMessage));

        var descriptor2 = new Mock<ICourierDescriptor>();
        descriptor2.SetupGet(m => m.Name)
            .Returns(new CourierName("Hello2"));
        descriptor2.SetupGet(m => m.CourierServiceType)
            .Returns(typeof(int));

        _descriptors.Add(descriptor.Object);
        _descriptors.Add(descriptor2.Object);

        var mockCourierService = new Mock<ICourierService>();
        mockCourierService.Setup(m => m.ScheduleAsync(It.IsAny<IMessage>(), It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Out.Success());

        var mockCourierService2 = new Mock<ICourierService>();
        mockCourierService2.Setup(m => m.DeliverAsync(It.IsAny<IMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Out.Success());

        _mockProvider.Setup(m => m.GetService(It.Is<Type>(t => t == typeof(IMessage))))
            .Returns(mockCourierService.Object);

        _mockProvider.Setup(m => m.GetService(It.Is<Type>(t => t == typeof(int))))
            .Returns(mockCourierService2.Object);

        // Act
        var output = await _dispatcher.DispatchAsync(new TestChildMessage(), TimeSpan.FromSeconds(1), new DispatchOptions()
        {
            DispatchStrategy = DispatchStrategy.FirstSuccess
        });

        // Assert
        Assert.True(output.IsSuccessful);

        mockCourierService.Verify(m => m.DeliverAsync(It.IsAny<IMessage>(), It.IsAny<CancellationToken>()), Times.Never);
        mockCourierService.Verify(m => m.ScheduleAsync(It.IsAny<IMessage>(), It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()), Times.Once);

        mockCourierService2.Verify(m => m.DeliverAsync(It.IsAny<IMessage>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task DispatchAsync_MultipleCouriers_NoFilter_FirstSuccess_FirstReturnsFailedUsesSecond_ReturnsSuccessfully()
    {
        // Arrange
        var descriptor = new Mock<ICourierDescriptor>();
        descriptor.SetupGet(m => m.Name)
            .Returns(new CourierName("Hello"));
        descriptor.SetupGet(m => m.CourierServiceType)
            .Returns(typeof(IMessage));

        var descriptor2 = new Mock<ICourierDescriptor>();
        descriptor2.SetupGet(m => m.Name)
            .Returns(new CourierName("Hello2"));
        descriptor2.SetupGet(m => m.CourierServiceType)
            .Returns(typeof(int));

        _descriptors.Add(descriptor.Object);
        _descriptors.Add(descriptor2.Object);

        var mockCourierService = new Mock<ICourierService>();
        mockCourierService.Setup(m => m.DeliverAsync(It.IsAny<IMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Out.InvalidRequest());

        var mockCourierService2 = new Mock<ICourierService>();
        mockCourierService2.Setup(m => m.DeliverAsync(It.IsAny<IMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Out.Success());

        _mockProvider.Setup(m => m.GetService(It.Is<Type>(t => t == typeof(IMessage))))
            .Returns(mockCourierService.Object);

        _mockProvider.Setup(m => m.GetService(It.Is<Type>(t => t == typeof(int))))
            .Returns(mockCourierService2.Object);

        // Act
        var output = await _dispatcher.DispatchAsync(new TestChildMessage(), TimeSpan.Zero, new DispatchOptions()
        {
            DispatchStrategy = DispatchStrategy.FirstSuccess
        });

        // Assert
        Assert.True(output.IsSuccessful);

        mockCourierService.Verify(m => m.ScheduleAsync(It.IsAny<IMessage>(), It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()), Times.Never);

        mockCourierService2.Verify(m => m.DeliverAsync(It.IsAny<IMessage>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DispatchAsync_MultipleCouriers_NoFilter_Broadcast_UsesAllCouriers_ReturnsSuccessfully()
    {
        // Arrange
        var descriptor = new Mock<ICourierDescriptor>();
        descriptor.SetupGet(m => m.Name)
            .Returns(new CourierName("Hello"));
        descriptor.SetupGet(m => m.CourierServiceType)
            .Returns(typeof(IMessage));

        var descriptor2 = new Mock<ICourierDescriptor>();
        descriptor2.SetupGet(m => m.Name)
            .Returns(new CourierName("Hello2"));
        descriptor2.SetupGet(m => m.CourierServiceType)
            .Returns(typeof(int));

        _descriptors.Add(descriptor.Object);
        _descriptors.Add(descriptor2.Object);

        var mockCourierService = new Mock<ICourierService>();
        mockCourierService.Setup(m => m.DeliverAsync(It.IsAny<IMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Out.Success());

        var mockCourierService2 = new Mock<ICourierService>();
        mockCourierService2.Setup(m => m.DeliverAsync(It.IsAny<IMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Out.Success());

        _mockProvider.Setup(m => m.GetService(It.Is<Type>(t => t == typeof(IMessage))))
            .Returns(mockCourierService.Object);

        _mockProvider.Setup(m => m.GetService(It.Is<Type>(t => t == typeof(int))))
            .Returns(mockCourierService2.Object);

        // Act
        var output = await _dispatcher.DispatchAsync(new TestChildMessage(), TimeSpan.Zero, new DispatchOptions()
        {
            DispatchStrategy = DispatchStrategy.Broadcast
        });

        // Assert
        Assert.True(output.IsSuccessful);

        mockCourierService.Verify(m => m.DeliverAsync(It.IsAny<IMessage>(), It.IsAny<CancellationToken>()), Times.Once);
        mockCourierService.Verify(m => m.ScheduleAsync(It.IsAny<IMessage>(), It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()), Times.Never);

        mockCourierService2.Verify(m => m.DeliverAsync(It.IsAny<IMessage>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DispatchAsync_MultipleCouriers_FilterToSpecificCouriers_Broadcast_UsesTargetedCouriers_ReturnsSuccessfully()
    {
        // Arrange
        var descriptor = new Mock<ICourierDescriptor>();
        descriptor.SetupGet(m => m.Name)
            .Returns(new CourierName("Hello"));
        descriptor.SetupGet(m => m.CourierServiceType)
            .Returns(typeof(IMessage));

        var descriptor2 = new Mock<ICourierDescriptor>();
        descriptor2.SetupGet(m => m.Name)
            .Returns(new CourierName("Hello2"));
        descriptor2.SetupGet(m => m.CourierServiceType)
            .Returns(typeof(int));

        var descriptor3 = new Mock<ICourierDescriptor>();
        descriptor2.SetupGet(m => m.Name)
            .Returns(new CourierName("Hello3"));
        descriptor2.SetupGet(m => m.CourierServiceType)
            .Returns(typeof(string));

        _descriptors.Add(descriptor.Object);
        _descriptors.Add(descriptor2.Object);
        _descriptors.Add(descriptor3.Object);

        var mockCourierService = new Mock<ICourierService>();
        mockCourierService.Setup(m => m.DeliverAsync(It.IsAny<IMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Out.Success());

        var mockCourierService2 = new Mock<ICourierService>();
        mockCourierService2.Setup(m => m.DeliverAsync(It.IsAny<IMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Out.Success());

        var mockCourierService3 = new Mock<ICourierService>();
        mockCourierService3.Setup(m => m.DeliverAsync(It.IsAny<IMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Out.Success());

        _mockProvider.Setup(m => m.GetService(It.Is<Type>(t => t == typeof(IMessage))))
            .Returns(mockCourierService.Object);

        _mockProvider.Setup(m => m.GetService(It.Is<Type>(t => t == typeof(int))))
            .Returns(mockCourierService2.Object);

        _mockProvider.Setup(m => m.GetService(It.Is<Type>(t => t == typeof(string))))
            .Returns(mockCourierService3.Object);

        // Act
        var output = await _dispatcher.DispatchAsync(new TestChildMessage(), TimeSpan.Zero, new DispatchOptions()
        {
            DispatchStrategy = DispatchStrategy.Broadcast,
            TargetCouriers = ["Hello", "Hello3"]
        });

        // Assert
        Assert.True(output.IsSuccessful);

        mockCourierService.Verify(m => m.DeliverAsync(It.IsAny<IMessage>(), It.IsAny<CancellationToken>()), Times.Once);
        mockCourierService.Verify(m => m.ScheduleAsync(It.IsAny<IMessage>(), It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()), Times.Never);

        mockCourierService2.Verify(m => m.DeliverAsync(It.IsAny<IMessage>(), It.IsAny<CancellationToken>()), Times.Never);

        mockCourierService3.Verify(m => m.DeliverAsync(It.IsAny<IMessage>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion
}
