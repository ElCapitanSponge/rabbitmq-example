namespace RabbitmqExample.Common;

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

public interface IStructuredMessage
{
    public string MessageType { get; set; }
    public string Message { get; set; }
}

public class StructuredMessage : IStructuredMessage
{
    #region Properties

    public string MessageType { get; set; }
    public string Message { get; set; }

    #endregion // Properties
}
