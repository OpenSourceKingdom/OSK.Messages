using Moq;
using OSK.Messages.Abstractions;
using OSK.Messages.Messaging.Internal.Services;
using OSK.Messages.Messaging.UnitTests._Helpers;
using OSK.Operations.Outputs;

namespace OSK.Messages.Messaging.UnitTests.Internal.Services;

public class MessageCenterBuilderTests
{
    #region Variables

    private readonly MessageCenterBuilder _builder = new();

    #endregion

    #region WithOptions

    [Fact]
    public void WithOptions_NullOptionsConfiguration_ThrowsArgumentNullException()
    {
        // Arrange/Act/Assert
        Assert.Throws<ArgumentNullException>(() => _builder.WithOptions(null!));
    }

    [Fact]
    public void WithOptions_ValidOptions_ReturnsSuccessfully()
    {
        // Arrange/Act/Assert
        _builder.WithOptions(_ => { });
    }

    #endregion

    #region UseMiddleware

    [Fact]
    public void UseMiddleware_Null_ThrowsArgumentNullException()
    {
        // Arrange/Act/Assert
        Assert.Throws<ArgumentNullException>(() => _builder.UseMiddleware(null!));
    }

    [Fact]
    public void UseMiddleware_Valid_ReturnsSuccessfully()
    {
        // Arrange/Act/Assert
        _builder.UseMiddleware((_, _) => Task.FromResult(Out.Success()));
    }

    #endregion

    #region UseMiddleware(T)

    [Fact]
    public void UseMiddleware_T_Valid_ReturnsSuccessfully()
    {
        // Arrange/Act/Assert
        _builder.UseMiddleware<TestMiddleware>();
    }

    #endregion

    #region ConfigureMessageBox

    [Fact]
    public void ConfigureMessageBox_NullConfiguration_ThrowsArgumentNullException()
    {
        // Arrange/Act/Assert
        Assert.Throws<ArgumentNullException>(() => _builder.ConfigureMessageBox<IMessage>(null!));
    }

    [Fact]
    public void ConfigureMessageBox_ValidConfiguration_ReturnsSuccessfully()
    {
        // Arrange/Act/Assert
        _builder.ConfigureMessageBox<IMessage>(_ => { });
    }

    [Fact]
    public void ConfigureMessageBox_ValidConfiguration_DuplicateMessageBoxType_ReturnsSuccessfully()
    {
        // Arrange
        _builder.ConfigureMessageBox<IMessage>(_ => { });

        // Act/Assert
        _builder.ConfigureMessageBox<IMessage>(_ => { });
    }

    #endregion

    #region BuildMessageCenter

    [Fact]
    public void BuildMessageCenter_NullServiceProvider_ThrowsArgumentNullException()
    {
        // Arrange/Act/Assert
        Assert.Throws<ArgumentNullException>(() => _builder.BuildMessageCenter(null!));
    }

    [Fact]
    public void BuildMessageCenter_ValidConfiguration_ReturnsSuccessfully()
    {
        // Arrange
        _builder.ConfigureMessageBox<IMessage>(c => c.WithRecipient((_, _) => Task.FromResult(Out.Success())));
        // Test Duplicate
        _builder.ConfigureMessageBox<IMessage>(c => c.WithRecipient((_, _) => Task.FromResult(Out.Success())));

        _builder.ConfigureMessageBox<TestMessage>(c => c.WithRecipient((_, _) => Task.FromResult(Out.Success())));
        _builder.WithOptions(o => o.AllowInheritedMessageDelivery = true);

        var mockProvider = new Mock<IServiceProvider>();
        mockProvider.Setup(m => m.GetService(It.Is<Type>(t => t == typeof(IServiceProvider))))
            .Returns(mockProvider.Object);

        // Act
        var center = _builder.BuildMessageCenter(mockProvider.Object);

        // Assert
        Assert.NotNull(center);

        var details = center.GetRecipientDetails();

        Assert.Equal(2, details.Count());
    }

    #endregion
}
