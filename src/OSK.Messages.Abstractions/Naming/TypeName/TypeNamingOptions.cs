namespace OSK.Messages.Abstractions.Naming.TypeName;

/// <summary>
/// Provides a set of options to configure the output of the <see cref="PipelineTypeNamingStrategy"/>
/// </summary>
public class TypeNamingOptions
{
    /// <summary>
    /// Uses the standard short type name for the id generation.
    /// </summary>
    public bool UseShortName { get; set; }

    /// <summary>
    /// Uses lower case for the output id
    /// </summary>
    public bool UseLowerCase { get; set; }

    /// <summary>
    /// Reverses the default name scheme
    /// </summary>
    public bool MessageTypeNameFirst { get; set; }
}
