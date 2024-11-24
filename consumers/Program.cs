namespace RabbitmqExample.Consumers;

class Program
{
    private static ManualResetEvent _quitEvent = new ManualResetEvent(false);

    static void Main(string[] args)
    {
        RabbitmqExample.Consumers.Consumer consumer = new Consumer();

        consumer.AvailiableQueues.ForEach(queueName =>
        {
            consumer.InitialiseMessageConsumer<string>(
                queueName,
                message =>
                {
                    Console.WriteLine($"{queueName.ToUpper()} Received message: {message}");
                }
            );
        });

        Console.CancelKeyPress += (sender, eventArgs) =>
        {
            eventArgs.Cancel = true;
            _quitEvent.Set();
        };

        _quitEvent.WaitOne();
    }
}
