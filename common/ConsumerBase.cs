using System.Text;
using System.Text.Json;
using RabbitMQ.Client.Events;

public interface IConsumerBase
{
    public AsyncEventingBasicConsumer InitialiseMessageConsumer<T>(string queueName, Action<T> action);
}

public abstract class ConsumerBase : CommonBase, IConsumerBase
{
    #region Constructors

    public ConsumerBase()
        : base() { }

    #endregion // Constructors

    #region Fields

	private Dictionary<string, AsyncEventingBasicConsumer> _consumers = new Dictionary<string, AsyncEventingBasicConsumer>();

    #endregion // Fields

    #region Methods

    public AsyncEventingBasicConsumer InitialiseMessageConsumer<T>(string queueName, Action<T> action)
	{
		if (this.Consumers.ContainsKey(queueName))
		{
			throw new InvalidOperationException($"Consumer for queue {queueName} already exists.");
		}

		if (!this.Queues.ContainsKey(queueName))
		{
			throw new InvalidOperationException($"Queue {queueName} does not exist.");
		}

		AsyncEventingBasicConsumer consumer = new AsyncEventingBasicConsumer(this.Channel);
		this.Consumers.Add(queueName, consumer);
		consumer.ReceivedAsync += (sender, eventArgs) =>
		{
			byte[] body = eventArgs.Body.ToArray();
			string messageJson = Encoding.UTF8.GetString(body);

			if (string.IsNullOrWhiteSpace(messageJson))
			{
				throw new InvalidOperationException("Message is empty.");
			}

			T? message = JsonSerializer.Deserialize<T>(messageJson);

			if (message == null)
			{
				throw new InvalidOperationException("Message is null.");
			}

			return Task.Run(() => action(message));
		};

		return consumer;
	}

    #endregion // Methods

    #region Properties

	protected Dictionary<string, AsyncEventingBasicConsumer> Consumers => this._consumers;

    #endregion // Properties
}
