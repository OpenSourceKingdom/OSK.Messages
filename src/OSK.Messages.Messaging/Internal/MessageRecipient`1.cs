using OSK.Messages.Abstractions;
using OSK.Messages.Messaging.Models;

namespace OSK.Messages.Messaging.Internal;

internal class MessageBoxRecipient(IMessageRecipient handler, MessageDelegate messageDelegate)
{
    public IMessageRecipient Handler => handler;

    public MessageDelegate Delegate => messageDelegate;
}
