using System.Text;
using System.Text.Json;
using RabbitMQ.Client;

namespace RabbitmqExample.Common;

public interface IPublisherBase
{
    public Task SendMessage(IEnumerable<string> queueNames, string message);
    public Task SendMessage<T>(IEnumerable<string> queueNames, T message);
}

public abstract class PublisherBase : CommonBase, IPublisherBase
{
    #region Constructors

    public PublisherBase()
        : base() { }

    #endregion // Constructors

    #region Methods

    public async Task SendMessage(IEnumerable<string> queueNames, string message)
    {
        await this.SendMessage<string>(queueNames, message);
    }

    public async Task SendMessage<T>(IEnumerable<string> queueNames, T message)
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
            await this.DeclareQueueIfNotDeclared(queueName);
            await this.Channel.BasicPublishAsync(
                exchange: this.MessageExchange,
                routingKey: (this.MessageExchangeType == ExchangeType.Direct) ? queueName : string.Empty,
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
