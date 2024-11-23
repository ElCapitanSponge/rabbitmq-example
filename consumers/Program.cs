using RabbitmqExample.Common;

namespace RabbitmqExample.Consumers
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }
    }

    class Consumer : ConsumerBase
    {
        public Consumer()
            : base()
        {
            this.QueueNames = new List<string> { "test" };
        }
    }
}
