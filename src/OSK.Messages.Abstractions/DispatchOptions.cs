using System.Collections.Generic;

namespace OSK.Messages.Abstractions;

/// <summary>
/// Allows configuring of how a message is dispatched into the message system
/// </summary>
public class DispatchOptions
{
    #region Static

    /// <summary>
    /// A default dispatch configuration that will send a message to the first successfully sent courier
    /// </summary>
    public static DispatchOptions Default { get; } = new DispatchOptions()
    {
        TargetCouriers = [],
        DispatchStrategy = DispatchStrategy.FirstSuccess
    };

    #endregion

    #region Variables

    /// <summary>
    /// Designates the specific couriers that are allowed to be used for the dispatch. If empty or null, then all couriers will be used.
    /// </summary>
    public IList<CourierName> TargetCouriers { get; set; } = [];

    /// <summary>
    /// Determines the strategy that is used when sending a dispatch
    /// </summary>
    public DispatchStrategy DispatchStrategy { get; set; }

    #endregion
}
