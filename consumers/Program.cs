namespace RabbitmqExample.Consumers
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ManualResetEvent _quitEvent = new ManualResetEvent(false);
            Consumer consumer = new Consumer(new List<string> { "foo", "bar" });
            consumer.StartConsuming();

            Console.CancelKeyPress += (sender, eventArgs) =>
            {
                eventArgs.Cancel = true;
                _quitEvent.Set();
            };

            _quitEvent.WaitOne();
        }
    }
}
