using System.Text;
using System.Text.Json;
using RabbitMQ.Client;

abstract class PublisherBase : CommonBase
{
    #region Constructors

    public PublisherBase()
        : base() { }

    #endregion // Constructors

    #region Methods

    public async void PublishMessage<T>(string queueName, T message)
    {
        if (!this.Queues.ContainsKey(queueName))
        {
            throw new InvalidOperationException($"Queue {queueName} does not exist.");
        }

        string messageJson = JsonSerializer.Serialize(message);
        byte[] body = Encoding.UTF8.GetBytes(messageJson);

        await this.Channel.BasicPublishAsync(
            exchange: string.Empty,
            routingKey: queueName,
            body: body
        );
    }

    #endregion // Methods
}
