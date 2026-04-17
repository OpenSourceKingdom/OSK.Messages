using OSK.Messages.Abstractions;
using OSK.Messages.Messaging.Models;
using OSK.Messages.Messaging.Naming.TypeName;

namespace OSK.Messages.Messaging.UnitTests;

public class PipelineTypeNamingStrategyTests
{
    #region GetPipelineName

    [Fact]
    public void GetPipelineName_NullRecipientDetails_ThrowsArgumentNullException()
    {
        // Arrange
        var strategy = new PipelineTypeNamingStrategy();
        
        // Act/Assert
        Assert.Throws<ArgumentNullException>(() => strategy.GetPipelineName(null!));
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void GetPipelineName_NullTypeInformation_ThrowsArgumentNullException(bool useNullRecipient)
    {
        // Arrange
        var strategy = new PipelineTypeNamingStrategy();
        var details = useNullRecipient
            ? new MessageRecipientDetails(typeof(int), null!)
            : new MessageRecipientDetails(null!, typeof(int));

        // Act/Assert
        Assert.Throws<ArgumentNullException>(() => strategy.GetPipelineName(details));
    }

    [Fact]
    public void GetPipelineName_ValidDetails_UsesDefaultValues_ReturnsExpectedUpperCaseTypeName()
    {
        // Arrange
        var strategy = new PipelineTypeNamingStrategy();

        var recipientType = typeof(int);
        var messageType = typeof(IMessage);

        var expectedTypeName = $"{recipientType.FullName}:{messageType.FullName}";

        // Act
        var name = strategy.GetPipelineName(new MessageRecipientDetails(messageType, recipientType));

        // Assert
        Assert.Equal(expectedTypeName, name);
    }

    [Fact]
    public void GetPipelineName_ValidDetails_LowerCase_ReturnsExpectedLowerCaseTypeName()
    {
        // Arrange
        var strategy = new PipelineTypeNamingStrategy(new TypeNamingOptions()
        {
            UseLowerCase = true
        });

        var recipientType = typeof(int);
        var messageType = typeof(IMessage);

        var expectedTypeName = $"{recipientType.FullName}:{messageType.FullName}".ToLower();

        // Act
        var name = strategy.GetPipelineName(new MessageRecipientDetails(messageType, recipientType));

        // Assert
        Assert.Equal(expectedTypeName, name);
    }

    [Fact]
    public void GetPipelineName_ValidDetails_ShortName_ReturnsExpectedUpperCaseTypeName()
    {
        // Arrange
        var strategy = new PipelineTypeNamingStrategy(new TypeNamingOptions()
        {
            UseShortName = true
        });

        var recipientType = typeof(int);
        var messageType = typeof(IMessage);

        var expectedTypeName = $"{recipientType.Name}:{messageType.Name}";

        // Act
        var name = strategy.GetPipelineName(new MessageRecipientDetails(messageType, recipientType));

        // Assert
        Assert.Equal(expectedTypeName, name);
    }

    [Fact]
    public void GetPipelineName_ValidDetails_MessageTypeNameFirst_ReturnsExpectedTypeNameReversed()
    {
        // Arrange
        var strategy = new PipelineTypeNamingStrategy(new TypeNamingOptions()
        {
            MessageTypeNameFirst = true
        });

        var recipientType = typeof(int);
        var messageType = typeof(IMessage);

        var expectedTypeName = $"{messageType.FullName}:{recipientType.FullName}";

        // Act
        var name = strategy.GetPipelineName(new MessageRecipientDetails(messageType, recipientType));

        // Assert
        Assert.Equal(expectedTypeName, name);
    }

    #endregion
}
