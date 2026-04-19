using System;

namespace OSK.Messages.Abstractions.Naming.TypeName;

/// <summary>
/// A naming strategy that utilizes the message and recipient types to create a unique pipeline id
/// </summary>
public class PipelineTypeNamingStrategy(TypeNamingOptions? options = null) : IPipelineNamingStrategy
{
    #region PipelineNamingStrategy

    public string GetPipelineName(MessageRecipientDetails recipientDetails)
    {
        if (recipientDetails is not { MessageType: { }, RecipientType: { } })
        {
            throw new ArgumentNullException(nameof(recipientDetails), "One or more potential details was null (recipient details, message type, recipient type)");
        }

        var useFullName = !options?.UseShortName ?? true;
        var useLowerCase = options?.UseLowerCase ?? false;

        var recipientTypeName = useFullName
            ? recipientDetails.RecipientType.FullName
            : recipientDetails.RecipientType.Name;
        recipientTypeName = useLowerCase
            ? recipientTypeName.ToLower()
            : recipientTypeName;

        var messageTypeName = useFullName
            ? recipientDetails.MessageType.FullName
            : recipientDetails.MessageType.Name;
        messageTypeName = useLowerCase
            ? messageTypeName.ToLower()
            : messageTypeName;

        var reverseNameScheme = options?.MessageTypeNameFirst ?? false;        
        return reverseNameScheme
            ? $"{messageTypeName}:{recipientTypeName}"
            : $"{recipientTypeName}:{messageTypeName}";
    }

    #endregion
}
