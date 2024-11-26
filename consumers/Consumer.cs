using RabbitmqExample.Common;

namespace RabbitmqExample.Consumers;

class Consumer : ConsumerBase
{
    public Consumer()
        : base()
    {
        this.QueueNames = new List<string> { "hello" };
        this.InitializeQueue();
    }
}
