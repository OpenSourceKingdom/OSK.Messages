using Moq;
using OSK.Messages.Abstractions;
using OSK.Messages.Messaging.Internal.Services;
using OSK.Messages.Messaging.UnitTests._Helpers;
using OSK.Operations.Outputs;

namespace OSK.Messages.Messaging.UnitTests.Internal.Services;

public class MessageBoxConfiguratorTests
{
    #region Variables

    private readonly MessageBoxConfigurator<IMessage> _configurator = new();

    #endregion

    #region WithRecipient

    [Fact]
    public void WithRecipient_NullHandler_ThrowsArgumentNullException()
    {
        // Arrange/Act/Assert
        Assert.Throws<ArgumentNullException>(() => _configurator.WithRecipient(null!));
    }

    [Fact]
    public void WithRecipient_ValidHandler_ReturnsSuccessfully()
    {
        // Arrange/Act/Assert
        _configurator.WithRecipient(_ => Task.FromResult(Out.Success()));
    }

    #endregion

    #region WithRecipient(T)

    [Fact]
    public void WithRecipient_ValidRecipient_T_ReturnsSuccessfully()
    {
        // Arrange/Act/Assert
        _configurator.WithRecipient<TestRecipient>();
    }

    #endregion

    #region UseMiddleware

    [Fact]
    public void UseMiddleware_NullMiddleware_ThrowsArgumentNullException()
    {
        // Arrange/Act/Assert
        Assert.Throws<ArgumentNullException>(() => _configurator.UseMiddleware(null!));
    }

    [Fact]
    public void UseMiddleware_ValidMiddleware_ReturnsSuccessfully()
    {
        // Arrange/Act/Assert
        _configurator.UseMiddleware((_, _) => Task.FromResult(Out.Success()));
    }

    #endregion

    #region UseMiddleware(T)

    [Fact]
    public void UseMiddleware_ValidMiddleware_T_ReturnsSuccessfully()
    {
        // Arrange/Act/Assert
        _configurator.UseMiddleware<TestMessageMiddleware>();
    }

    #endregion

    #region BuildMessageBox

    [Fact]
    public void BuildMessageBox_NullServiceProvider_ThrowsArgumentNullException()
    {
        // Arrange/Act/Assert
        Assert.Throws<ArgumentNullException>(() => _configurator.BuildMessageBox([], null!));
    }

    [Fact]
    public void BuildMessageBox_NoHandlersRegistered_ThrowsInvalidOperationException()
    {
        // Arrange
        var mockProvider = new Mock<IServiceProvider>();

        // Act/Assert
        Assert.Throws<InvalidOperationException>(() => _configurator.BuildMessageBox([], mockProvider.Object));
    }

    [Fact]
    public async Task BuildMessageBox_SingleTypedRecipient_ValidConfiguration_ReturnsSuccessfully()
    {
        // Arrange
        var usedHandler = false;
        var usedGenericMiddleware = false;
        var usedTypedMiddleware = false;
        var usedGlobalMiddleware = false;

        var mockProvider = new Mock<IServiceProvider>();

        mockProvider.Setup(m => m.GetService(It.Is<Type>(t => t == typeof(TestRecipient))))
            .Returns(new TestRecipient(_ => usedHandler = true));

        mockProvider.Setup(m => m.GetService(It.Is<Type>(t => t == typeof(TestMessageMiddleware))))
            .Returns(new TestMessageMiddleware(_ => usedTypedMiddleware = true));

        _configurator.WithRecipient<TestRecipient>();
        _configurator.UseMiddleware((context, next) =>
        {
            usedGenericMiddleware = true;
            return next(context);
        });
        _configurator.UseMiddleware<TestMessageMiddleware>();

        // Act
        var box = _configurator.BuildMessageBox([new TestMiddleware(_ => usedGlobalMiddleware = true)], mockProvider.Object);

        await box.DeliverMessageAsync(Mock.Of<IMessage>(), mockProvider.Object);

        // Assert
        Assert.NotNull(box);
        Assert.True(usedGlobalMiddleware);
        Assert.True(usedGenericMiddleware);
        Assert.True(usedTypedMiddleware);
        Assert.True(usedGlobalMiddleware);
        Assert.True(usedHandler);
    }

    [Fact]
    public async Task BuildMessageBox_MultipleTypedRecipients_ValidConfiguration_ReturnsSuccessfully()
    {
        // Arrange
        var usedHandler1 = false;
        var usedHandler2 = false;

        var usedGenericMiddleware = false;
        var usedTypedMiddleware = false;
        var usedGlobalMiddleware = false;

        var mockProvider = new Mock<IServiceProvider>();

        mockProvider.Setup(m => m.GetService(It.Is<Type>(t => t == typeof(TestRecipient))))
            .Returns(new TestRecipient(_ =>
            {
                usedHandler1 = true;
            }));

        mockProvider.Setup(m => m.GetService(It.Is<Type>(t => t == typeof(TestMessageMiddleware))))
            .Returns(new TestMessageMiddleware(_ => usedTypedMiddleware = true));

        _configurator.WithRecipient<TestRecipient>();
        _configurator.WithRecipient(_ =>
        {
            usedHandler2 = true;
            return Task.FromResult(Out.Success());
        });


        _configurator.UseMiddleware((context, next) =>
        {
            usedGenericMiddleware = true;
            return next(context);
        });
        _configurator.UseMiddleware<TestMessageMiddleware>();

        // Act
        var box = _configurator.BuildMessageBox([new TestMiddleware(_ => usedGlobalMiddleware = true)], mockProvider.Object);

        await box.DeliverMessageAsync(Mock.Of<IMessage>(), mockProvider.Object);

        // Assert
        Assert.NotNull(box);
        Assert.True(usedGlobalMiddleware);
        Assert.True(usedGenericMiddleware);
        Assert.True(usedTypedMiddleware);
        Assert.True(usedGlobalMiddleware);
        Assert.True(usedHandler1);
        Assert.True(usedHandler2);
    }

    #endregion
}
