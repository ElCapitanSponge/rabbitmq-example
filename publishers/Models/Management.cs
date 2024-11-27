using RabbitmqExample.Common;

namespace RabbitmqExample.Publishers.Models;

public class Management : CommonBase
{
    #region Constructors

    public Management()
        : base() { }

    #endregion // Constructors

    #region Properties

    protected override string AdminPassword => "guest";
    protected override string AdminPort => "15672";
    protected override string AdminUser => "guest";
    protected override string HostName => "localhost";

    #endregion // Properties
}
