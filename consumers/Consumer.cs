using RabbitmqExample.Common;

namespace RabbitmqExample.Consumers;

public class Consumer : ConsumerBase
{
    #region Constructors

    public Consumer()
        : base()
	{
		this.QueueNames = new List<string>() { "test" };
	}

    #endregion // Constructors
}
