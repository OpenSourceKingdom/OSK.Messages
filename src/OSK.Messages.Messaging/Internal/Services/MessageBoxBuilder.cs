using System;
using System.Collections.Generic;
using OSK.Messages.Messaging.Ports;

namespace OSK.Messages.Messaging.Internal.Services;

internal abstract class MessageBoxBuilder
{
    public abstract MessageBox BuildMessageBox(IEnumerable<IMessageMiddleware> globalMiddlewares, IServiceProvider services);
}
