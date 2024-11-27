using RabbitmqExample.Common;

namespace RabbitmqExample.Consumers;

public class Consumer : ConsumerBase
{
    #region Constructors

    public Consumer(List<string> queuesNames)
        : base(queuesNames) { }

    #endregion // Constructors

    #region Properties

    protected override string HostName => "localhost";

    #endregion // Properties
}
