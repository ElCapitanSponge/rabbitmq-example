using System.Text;
using System.Text.Json;
using RabbitMQ.Client;

namespace RabbitmqExample.Common;

public interface IPublisherBase
{
	public void PublishMessage(string queueName, string message);
}

public abstract class PublisherBase : CommonBase, IPublisherBase
{
    #region Constructors

    public PublisherBase(List<string> queueNames)
        : base()
	{
		this.QueueNames = queueNames;
	}

    #endregion // Constructors

    #region Methods

    public async void PublishMessage(string queueName, string message)
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
