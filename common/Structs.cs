namespace RabbitmqExample.Common;

public interface IChannelDetail
{
    public string? connection_name { get; set; }
    public string? name { get; set; }
    public string? node { get; set; }
    public int? number { get; set; }
    public string? peer_host { get; set; }
    public int? peer_port { get; set; }
    public string? user { get; set; }
}

public class ChannelDetail : IChannelDetail
{
    public string? connection_name { get; set; }
    public string? name { get; set; }
    public string? node { get; set; }
    public int? number { get; set; }
    public string? peer_host { get; set; }
    public int? peer_port { get; set; }
    public string? user { get; set; }
}

public interface IConsumerDetail
{
    public ChannelDetail? channel_details { get; set; }
}

public class ConsumerDetail : IConsumerDetail
{
    public ChannelDetail? channel_details { get; set; }
}

public interface IQueueInfo
{
    public string? Name { get; set; }
}

public class QueueInfo : IQueueInfo
{
    #region Properties

    public string? Name { get; set; }

    #endregion // Properties
}

public interface IQueueConnections
{
    public List<ConsumerDetail>? consumer_details { get; set; }
}

public class QueueConnections : IQueueConnections
{
    public List<ConsumerDetail>? consumer_details { get; set; }
}

public interface IStructuredMessage
{
    public string? MessageType { get; set; }
    public string? Message { get; set; }
}

public class StructuredMessage : IStructuredMessage
{
    #region Properties

    public string? MessageType { get; set; }
    public string? Message { get; set; }

    #endregion // Properties
}
