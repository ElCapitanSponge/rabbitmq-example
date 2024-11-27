using System.Text;
using RabbitMQ.Client;

namespace RabbitmqExample.Common;

public interface IPublisherBase { }

public abstract class PublisherBase : CommonBase, IPublisherBase
{
    #region Constructors

    public PublisherBase(List<string> queueNames)
        : base() { }

    #endregion // Constructors

    #region Methods

    public void SendMessage(IEnumerable<string> queueNames, string message)
    {
        var body = Encoding.UTF8.GetBytes(message);

        foreach (var queueName in queueNames)
        {
            this.DeclareQueueIfNotDeclared(queueName);
            this.Channel.BasicPublishAsync(
                exchange: string.Empty,
                routingKey: queueName,
                body: body
            );

            Console.WriteLine($" [{queueName}] Sent {message}");
        }
    }

    #endregion // Methods
}
