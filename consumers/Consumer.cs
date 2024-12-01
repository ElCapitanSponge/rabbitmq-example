using RabbitMQ.Client;
using RabbitmqExample.Common;

namespace RabbitmqExample.Consumers;

public class Consumer : ConsumerBase
{
    #region Constructors

    public Consumer(string name, List<string> queuesNames)
        : base(name, queuesNames) { }

    public Consumer(string name, List<string> queueNames, IChannel channel, IConnection connection)
        : base(name, queueNames, channel, connection) { }

    #endregion // Constructors

    #region Properties

    protected override string AdminPassword => "guest";
    protected override string AdminPort => "15672";
    protected override string AdminUser => "guest";
    protected override string HostName => "localhost";

    #endregion // Properties
}
