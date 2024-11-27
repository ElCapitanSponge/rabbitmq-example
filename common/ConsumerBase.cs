using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitmqExample.Common;

public interface IConsumerBase
{
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
		foreach (var queueName in this.SpecifiedQueues)
		{
			this.DeclareQueueIfNotDeclared(queueName);
			var consumer = new AsyncEventingBasicConsumer(this.Channel);
			consumer.ReceivedAsync += (model, ea) =>
			{
				var body = ea.Body.ToArray();
				var message = Encoding.UTF8.GetString(body);
				Console.WriteLine($" [{queueName}] Received {message}");
				return Task.CompletedTask;
			};

			this.Channel.BasicConsumeAsync(queue: queueName, autoAck: true, consumer: consumer).Wait();
		}
	}

    #endregion // Methods

    #region Properties

	public List<string> SpecifiedQueues => this._specifiedQueues;

    #endregion // Properties
}
