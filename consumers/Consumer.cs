using RabbitmqExample.Common;

namespace RabbitmqExample.Consumers;

public class Consumer : ConsumerBase
{
    #region Constructors

    public Consumer(string name, List<string> queuesNames)
        : base(name, queuesNames) { }

    #endregion // Constructors

    #region Properties

	protected override string AdminPassword => "guest";
    protected override string AdminPort => "15672";
    protected override string AdminUser => "guest";
    protected override string HostName => "localhost";

    #endregion // Properties
}
