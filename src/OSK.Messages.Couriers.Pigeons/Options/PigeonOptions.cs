namespace OSK.Messages.Couriers.Pigeons.Options;

/// <summary>
/// Configures the process for how local pigeon messaging is operated
/// </summary>
public class PigeonOptions
{
    /// <summary>
    /// Runs the mwessages via background threads rather than directly in-process
    /// </summary>
    public bool UseBackgroundMessaging { get; set; }
}
