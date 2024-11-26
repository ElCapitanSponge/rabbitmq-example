using System.Text;
using System.Text.Json;

namespace RabbitmqExample.Consumers;

class Program
{
    private static ManualResetEvent _quitEvent = new ManualResetEvent(false);

    static void Main(string[] args)
    {
        RabbitmqExample.Consumers.Consumer consumer = new Consumer();

        consumer.AvailiableQueues.ForEach(queueName =>
        {
            consumer.InitialiseMessageConsumer(queueName);
            consumer.Consumers[queueName].ReceivedAsync += (sender, eventArgs) =>
            {
                byte[] body = eventArgs.Body.ToArray();
                string message = Encoding.UTF8.GetString(body);
                var messageParsed = JsonSerializer.Deserialize<string>(message);
                Console.WriteLine($" [{queueName}] Received {messageParsed}");
                return Task.CompletedTask;
            };
        });

        Console.CancelKeyPress += (sender, eventArgs) =>
        {
            eventArgs.Cancel = true;
            _quitEvent.Set();
        };

        _quitEvent.WaitOne();
    }
}
