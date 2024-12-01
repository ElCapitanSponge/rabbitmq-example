using System.Text;
using System.Text.Json;
using RabbitMQ.Client;

namespace RabbitmqExample.Common;

public interface IPublisherBase
{
    public void SendMessage(IEnumerable<string> queueNames, string message);
    public void SendMessage<T>(IEnumerable<string> queueNames, T message);
}

public abstract class PublisherBase : CommonBase, IPublisherBase
{
    #region Constructors

    public PublisherBase()
        : base() { }

    #endregion // Constructors

    #region Methods

    public void SendMessage(IEnumerable<string> queueNames, string message)
    {
        this.SendMessage<string>(queueNames, message);
    }

    public void SendMessage<T>(IEnumerable<string> queueNames, T message)
    {
        string serialisedMessage = JsonSerializer.Serialize(message);
        StructuredMessage structuredMessage = new StructuredMessage
        {
            MessageType = typeof(T).FullName ?? string.Empty,
            Message = serialisedMessage,
        };

        byte[] body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(structuredMessage));

        foreach (string queueName in queueNames)
        {
            this.DeclareQueueIfNotDeclared(queueName);
            this.Channel.BasicPublishAsync(
                exchange: $"exchange-{queueName}",
                routingKey: $"{queueName}-key",
                body: body
            );

            Console.WriteLine($"[{queueName}] Sent structured message: {message}");
            Console.WriteLine(
                $"[{queueName}] Sent message: {structuredMessage.MessageType} ~ {structuredMessage.Message}"
            );
        }
    }

    #endregion // Methods
}
