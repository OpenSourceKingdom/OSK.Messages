using System.Threading.Tasks;
using OSK.Messages.Abstractions;

namespace OSK.Messages.Messaging.Models;

public delegate Task MessageDelegate(MessageContext context);
