using System.Collections.Generic;

namespace OSK.Messages.Abstractions;

public class DispatchOptions
{
    #region Static

    public static DispatchOptions Default { get; } = new DispatchOptions();

    #endregion

    #region Variables

    public IList<CourierName> TargetCouriers { get; set; } = [];

    public DispatchStrategy DispatchStrategy { get; set; } = DispatchStrategy.FirstSuccess;

    #endregion
}
