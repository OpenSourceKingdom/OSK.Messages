using Moq;
using OSK.Messages.Messaging.Internal;
using OSK.Messages.Messaging.Internal.Services;
using OSK.Messages.Messaging.Models;
using OSK.Messages.Messaging.Options;
using OSK.Messages.Messaging.UnitTests._Helpers;
using OSK.Operations.Outputs;

namespace OSK.Messages.Messaging.UnitTests.Internal.Services;

public class MessageCenterTests
{
    #region Variables

    private readonly MessagingOptions _options;
    private readonly List<MessageBox> _boxes = [];

    private readonly MessageCenter _messageCenter;

    #endregion

    #region Constructors

    public MessageCenterTests()
    {
        _options = new MessagingOptions();

        _messageCenter = new(_boxes, Mock.Of<IServiceProvider>(), _options);
    }

    #endregion

    #region GetRecipientDetails

    [Fact]
    public void GetRecipientDetails_NoRecipients_ReturnsEmptyList()
    {
        // Arrange/Act/Assert
        Assert.Empty(_messageCenter.GetRecipientDetails());
    }

    [Fact]
    public void GetRecipientDetails_HasMessageBoxes_ReturnsDetailsOfRecipients()
    {
        // Arrange
        MessageDelegate recipientDelegate = _ => Task.FromResult(Out.Success());
        _boxes.Add(new MessageBox(typeof(TestMessage), [new MessageBoxRecipient(new TestRecipient<TestMessage>(_ => { }), recipientDelegate)]));
        _boxes.Add(new MessageBox(typeof(TestMessage), [new MessageBoxRecipient(new TestRecipient<TestMessage>(_ => { }), recipientDelegate), new MessageBoxRecipient(new TestRecipient<TestMessage>(_ => { }), recipientDelegate)]));

        // Act
        var details = _messageCenter.GetRecipientDetails();

        // Assert
        Assert.NotEmpty(details);
        Assert.Equal(3, details.Count());

        foreach (var detail in details)
        {
            Assert.Equal(typeof(TestRecipient<TestMessage>), detail.RecipientType);
            Assert.Equal(typeof(TestMessage), detail.MessageType);
        }
    }

    #endregion

    #region ReceiveAsync

    [Fact]
    public async Task ReceiveAsync_NoMessageBoxesForMessage_ReturnsSuccessfully()
    {
        // Arrange/Act
        var receiveOutput = await _messageCenter.ReceiveAsync(new TestMessage());

        // Assert
        Assert.True(receiveOutput.IsSuccessful);
    }

    [Fact]
    public async Task ReceiveAsync_MultipleBoxes_InheritedMessage_InheritanceMessagingDisabled_HandledBySingleBox_ReturnsSuccessfully()
    {
        // Arrange
        _options.AllowInheritedMessageDelivery = false;

        var usedParentMessage = false;
        var usedInheritedMessage = false;

        MessageDelegate recipientDelegate1 = _ =>
        {
            usedParentMessage = true;
            return Task.FromResult(Out.Success());
        };
        _boxes.Add(new MessageBox(typeof(TestMessage), [new MessageBoxRecipient(new TestRecipient<TestMessage>(_ => { /* Delegate assumed to trigger recipient, so just using delegate */ }), recipientDelegate1)]));

        MessageDelegate recipientDelegate2 = _ =>
        {
            usedInheritedMessage = true;
            return Task.FromResult(Out.Success());
        };
        _boxes.Add(new MessageBox(typeof(TestChildMessage), [new MessageBoxRecipient(new TestRecipient<TestChildMessage>(_ => { /* Delegate assumed to trigger recipient, so just using delegate */ }), recipientDelegate2)]));

        // Act
        var output = await _messageCenter.ReceiveAsync(new TestChildMessage());

        // Assert
        Assert.True(output.IsSuccessful);
        Assert.True(usedInheritedMessage);
        Assert.False(usedParentMessage);
    }

    [Fact]
    public async Task ReceiveAsync_MultipleBoxes_InheritedMessage_InheritanceMessagingEnabled_HandledByInheritedBox_ReturnsSuccessfully()
    {
        // Arrange
        _options.AllowInheritedMessageDelivery = true;

        var usedParentMessage = false;
        var usedInheritedMessage = false;

        MessageDelegate recipientDelegate1 = _ =>
        {
            usedParentMessage = true;
            return Task.FromResult(Out.Success());
        };
        _boxes.Add(new MessageBox(typeof(TestMessage), [new MessageBoxRecipient(new TestRecipient<TestMessage>(_ => { /* Delegate assumed to trigger recipient, so just using delegate */ }), recipientDelegate1)]));

        MessageDelegate recipientDelegate2 = _ =>
        {
            usedInheritedMessage = true;
            return Task.FromResult(Out.Success());
        };
        _boxes.Add(new MessageBox(typeof(TestMessage), [new MessageBoxRecipient(new TestRecipient<TestChildMessage>(_ => { /* Delegate assumed to trigger recipient, so just using delegate */ }), recipientDelegate2)]));

        // Act
        var output = await _messageCenter.ReceiveAsync(new TestChildMessage());

        // Assert
        Assert.True(output.IsSuccessful);
        Assert.True(usedInheritedMessage);
        Assert.True(usedParentMessage);
    }

    #endregion
}
