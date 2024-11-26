using RabbitMQ.Client.Events;

namespace RabbitmqExample.Common;

public interface IConsumerBase
{
	public void InitialiseMessageConsumer(string queueName);
	public Dictionary<string, AsyncEventingBasicConsumer> Consumers { get; }
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

    public void InitialiseMessageConsumer(string queueName)
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
	}

    #endregion // Methods

    #region Properties

	public Dictionary<string, AsyncEventingBasicConsumer> Consumers => this._consumers;

    #endregion // Properties
}
