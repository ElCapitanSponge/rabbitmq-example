using RabbitMQ.Client;

namespace RabbitmqExample.Common;

public interface ICommonBase { }

public abstract class CommonBase : ICommonBase
{
    #region Constructors

    public CommonBase()
    {
        this._factory = new ConnectionFactory { HostName = "localhost" };
        this._connection = this.Factory.CreateConnectionAsync().Result;
        this._channel = this.Connection.CreateChannelAsync().Result;

        this.InitializeQueue();
    }

    #endregion // Constructors

    #region Fields

    private IChannel _channel;
    private IConnection _connection;
    private ConnectionFactory _factory;
    private Dictionary<string, QueueDeclareOk> _queues = new Dictionary<string, QueueDeclareOk>();
    private List<string> _queueNames = new List<string>();

    #endregion // Fields

    #region Methods

    protected void InitializeQueue()
    {
        this.QueueNames.ForEach(async queueName =>
        {
            if (this.Queues.ContainsKey(queueName))
            {
                return;
            }

            QueueDeclareOk? queue = await this.Channel.QueueDeclareAsync(
                queue: queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );

            if (queue != null)
            {
                this.Queues.Add(queueName, queue);
            }
        });
    }

    #endregion // Methods

    #region Properties

    public List<string> AvailiableQueues => this.Queues.Keys.ToList();

    protected IChannel Channel
    {
        get => this._channel;
        private set => this._channel = value;
    }

    protected IConnection Connection
    {
        get => this._connection;
        private set => this._connection = value;
    }

    protected ConnectionFactory Factory
    {
        get => this._factory;
        private set => this._factory = value;
    }

    protected List<string> QueueNames
    {
        get => this._queueNames;
        set => this._queueNames = value;
    }

    protected Dictionary<string, QueueDeclareOk> Queues => this._queues;

    #endregion // Properties
}
