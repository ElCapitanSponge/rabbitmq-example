using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitmqExample.Common;

public interface IConsumerBase
{
    public void StartConsuming();
    public List<string> SpecifiedQueues { get; }
}

public abstract class ConsumerBase : CommonBase, IConsumerBase
{
    #region Constructors

    public ConsumerBase(List<string> queuesNames)
        : base()
    {
        this._specifiedQueues = queuesNames;
    }

    #endregion // Constructors

    #region Fields

    private List<string> _specifiedQueues;

    #endregion // Fields

    #region Methods

    public void StartConsuming()
    {
        foreach (string queueName in this.SpecifiedQueues)
        {
            this.DeclareQueueIfNotDeclared(queueName);
            AsyncEventingBasicConsumer consumer = new AsyncEventingBasicConsumer(this.Channel);
            consumer.ReceivedAsync += (model, ea) =>
            {
                byte[] body = ea.Body.ToArray();
                string message = Encoding.UTF8.GetString(body);

                try
                {
                    StructuredMessage? decodedBaseMessage =
                        JsonSerializer.Deserialize<StructuredMessage>(message);

                    if (
                        decodedBaseMessage == null
                        || decodedBaseMessage.Message == null
                        || decodedBaseMessage.MessageType == null
                    )
                    {
                        Console.WriteLine(
                            $" [{queueName}] Could not deserialise message: {message}"
                        );
                        return Task.CompletedTask;
                    }

                    Type? messageType = Type.GetType(decodedBaseMessage.MessageType);

                    if (messageType == null)
                    {
                        Console.WriteLine(
                            $"[{queueName}] could not determine type for message data"
                        );
                        return Task.CompletedTask;
                    }

                    object? deserializedMessage = JsonSerializer.Deserialize(
                        decodedBaseMessage.Message,
                        messageType
                    );

                    if (deserializedMessage == null)
                    {
                        Console.WriteLine(
                            $" [{queueName}] Could not deserialise message: {decodedBaseMessage.Message}"
                        );
                        return Task.CompletedTask;
                    }

                    Console.WriteLine($" [{queueName}] Received {deserializedMessage.ToString()}");
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($" [{queueName}] Error: {ex.Message}");
                    this.Channel.BasicNackAsync(
                        deliveryTag: ea.DeliveryTag,
                        multiple: false,
                        requeue: false
                    );
                }
                return Task.CompletedTask;
            };

            this.Channel.BasicConsumeAsync(queue: queueName, autoAck: true, consumer: consumer)
                .Wait();
            Console.WriteLine($" [{queueName}] Waiting for messages.");
        }
    }

    #endregion // Methods

    #region Properties

    public List<string> SpecifiedQueues => this._specifiedQueues;

    #endregion // Properties
}
