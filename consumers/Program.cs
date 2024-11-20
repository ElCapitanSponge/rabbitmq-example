using System;

namespace RabbitmqExample.Consumers;

public class Program
{
    public static void Main(string[] args)
    {
        RabbitmqExample.Consumers.Consumer consumer = new RabbitmqExample.Consumers.Consumer();

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
    }
}
