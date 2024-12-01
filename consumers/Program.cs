using RabbitMQ.Client;

namespace RabbitmqExample.Consumers
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ManualResetEvent _quitEvent = new ManualResetEvent(false);

            Consumer fiz = new Consumer("Fiz", new List<string> { "fiz" });
            fiz.StartConsuming().Wait();
            IChannel channel = fiz.GetChannel();
            IConnection connection = fiz.GetConnection();

            Consumer buz = new Consumer("Buz", new List<string> { "buz" }, channel, connection);
            buz.StartConsuming().Wait();

            Consumer wiz = new Consumer("Wiz", new List<string> { "wiz" }, channel, connection);
            wiz.StartConsuming().Wait();

            Consumer fizBuz = new Consumer("FizBuz", new List<string> { "fiz", "buz" }, channel, connection);
            fizBuz.StartConsuming().Wait();

            Consumer fizWiz = new Consumer("FizWiz", new List<string> { "fiz", "wiz" }, channel, connection);
            fizWiz.StartConsuming().Wait();

            Consumer buzWiz = new Consumer("BuzWiz", new List<string> { "buz", "wiz" }, channel, connection);
            buzWiz.StartConsuming().Wait();

            Consumer fizBuzWiz = new Consumer(
                "FizBuzWiz",
                new List<string> { "fiz", "buz", "wiz" },
                channel,
                connection
            );
            fizBuzWiz.StartConsuming().Wait();

            Console.CancelKeyPress += (sender, eventArgs) =>
            {
                eventArgs.Cancel = true;
                _quitEvent.Set();
            };

            _quitEvent.WaitOne();
        }
    }
}
