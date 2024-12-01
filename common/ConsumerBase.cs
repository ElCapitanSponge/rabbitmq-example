using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitmqExample.Common;

public interface IConsumerBase
{
    public Task StartConsuming();
    public List<string> SpecifiedQueues { get; }
}

public abstract class ConsumerBase : CommonBase, IConsumerBase
{
    #region Constructors

    public ConsumerBase(string name, List<string> queueNames)
        : base()
    {
        this._name = name;
        this._specifiedQueues = queueNames;
    }

    public ConsumerBase(string name, List<string> queueNames, IChannel channel, IConnection connection) : base(channel, connection)
    {
        this._name = name;
        this._specifiedQueues = queueNames;
    }

    #endregion // Constructors

    #region Fields

    private string _name;
    private List<string> _specifiedQueues;

    #endregion // Fields

    #region Methods

    public async Task StartConsuming()
    {
        foreach (string queueName in this.SpecifiedQueues)
        {
            await this.DeclareQueueIfNotDeclared(queueName);
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
                            $"[{this.Name}][{queueName}] Could not deserialise message: {message}"
                        );
                        return Task.CompletedTask;
                    }

                    Type? messageType = Type.GetType(decodedBaseMessage.MessageType);

                    if (messageType == null)
                    {
                        Console.WriteLine(
                            $"[{this.Name}][{queueName}] could not determine type for message data"
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
                            $"[{this.Name}][{queueName}] Could not deserialise message: {decodedBaseMessage.Message}"
                        );
                        return Task.CompletedTask;
                    }

                    Console.WriteLine(
                        $"[{this.Name}][{queueName}] Received {deserializedMessage.ToString()}"
                    );
                    this.Channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
                    return Task.CompletedTask;
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"[{this.Name}][{queueName}] Error: {ex.Message}");
                    this.Channel.BasicNackAsync(
                        deliveryTag: ea.DeliveryTag,
                        multiple: false,
                        requeue: false
                    );
                    return Task.FromException(ex);
                }
            };

            //await this.Channel.BasicQosAsync(0, 1, false);

            await this.Channel.BasicConsumeAsync(queue: queueName, autoAck: false, consumer: consumer);
            Console.WriteLine($"[{this.Name}][{queueName}] Waiting for messages.");
        }
    }

    #endregion // Methods

    #region Properties

    private string Name => this._name;
    public List<string> SpecifiedQueues => this._specifiedQueues;

    #endregion // Properties
}
