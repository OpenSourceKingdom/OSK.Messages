using System;

namespace OSK.Messages.Messaging.Options;

public class MessagingOptions
{
    #region Variables

    private int _maxParallelRecipients = 1;

    #endregion

    #region Variables

    public int MaxParallelRecipients {
        get => _maxParallelRecipients;
        set
        {
            if (_maxParallelRecipients < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(MaxParallelRecipients), "Must be a postive, non-zero integer.");
            }

            _maxParallelRecipients = value;
        } 
    }

    public bool AllowInheritedRecipients { get; set; }

    #endregion
}
