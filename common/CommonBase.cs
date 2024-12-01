using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using RabbitMQ.Client;

namespace RabbitmqExample.Common;

public interface ICommonBase
{
    public Task<bool> CreateQueue(string queueName);
    public void Dispose();
    public IConnection GetConnection();
    public IChannel GetChannel();
    public Task RefreshQueues();
    public Task RemoveAllQueues();
    public HashSet<KeyValuePair<string, bool>> Queues { get; }
}

public abstract class CommonBase : ICommonBase
{
    #region Constructors

    public CommonBase()
    {
        var factory = new ConnectionFactory { HostName = this.HostName };
        this._connection = factory.CreateConnectionAsync().Result;
        this._channel = this.Connection.CreateChannelAsync().Result;
        this.DeclareExchanges().Wait();
        this.LoadQueues().Wait();
        this.CreateDeadLetterExchange().Wait();
    }

    public CommonBase(IChannel channel, IConnection connection)
    {
        this._channel = channel;
        this._connection = connection;
        this.LoadQueues().Wait();
    }

    #endregion // Constructors

    #region Fields

    private IChannel _channel;
    private IConnection _connection;
    private readonly HashSet<KeyValuePair<string, bool>> _queues =
        new HashSet<KeyValuePair<string, bool>>();

    #endregion // Fields

    #region Methods

    public async Task<bool> CreateQueue(string queueName)
    {
        await this.DeclareQueueIfNotDeclared(queueName);
        bool exists = this.IsInQueue(queueName);
        return exists;
    }

    private async Task DeclareExchanges()
    {
        await this.Channel.ExchangeDeclareAsync(
            exchange: $"exchange_{this.DeadLetterExchange}",
            type: ExchangeType.Direct
        );
        await this.Channel.ExchangeDeclareAsync(
            exchange: this.MessageExchange,
            type: this.MessageExchangeType
        );
    }

    protected async Task DeclareQueueIfNotDeclared(string queueName, bool isDeadLetter = false)
    {
        if (!this.IsInQueue(queueName))
        {
            IDictionary<string, object?>? args = null;
            if (!isDeadLetter)
            {
                args = new Dictionary<string, object?>
                {
                    { "x-dead-letter-exchange", $"exchange-{this.DeadLetterExchange}" },
                    { "x-dead-letter-routing-key", $"{this.DeadLetterExchange}-key" },
                };
            }
            await this.Channel.QueueDeclareAsync(
                    queue: queueName,
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: args
                );
            if (isDeadLetter)
            {
                await this.Channel.QueueBindAsync(
                    queue: queueName,
                    exchange: $"exchange_{this.DeadLetterExchange}",
                    routingKey: $"{this.DeadLetterExchange}-key"
                );
            }
            else
            {
                await this.Channel.QueueBindAsync(
                    queue: queueName,
                    exchange: this.MessageExchange,
                    routingKey: (this.MessageExchangeType == ExchangeType.Direct) ? queueName : string.Empty
                );
            }
            this.Queues.Add(new KeyValuePair<string, bool>(queueName, isDeadLetter));
        }
    }

    public void Dispose()
    {
        this.Channel?.CloseAsync();
        this.Connection?.CloseAsync();
    }

    private async Task CreateDeadLetterExchange()
    {
        if (!this.IsInQueue(this.DeadLetterExchange))
        {
            await this.DeclareQueueIfNotDeclared(this.DeadLetterExchange, true);
        }
    }

    public IChannel GetChannel() => this.Channel;

    public IConnection GetConnection() => this.Connection;

    private bool IsInQueue(string queueName)
    {
        return this.Queues.Where(q => q.Key == queueName).Count() > 0;
    }

    private async Task LoadQueues()
    {
        var httpClient = new HttpClient();
        var credentials = Convert.ToBase64String(
            Encoding.ASCII.GetBytes($"{this.AdminUser}:{this.AdminPassword}")
        );
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Basic",
            credentials
        );

        var queues = await httpClient.GetFromJsonAsync<List<QueueInfo>>(
            $"http://{this.HostName}:{this.AdminPort}/api/queues"
        );
        if (queues != null)
        {
            foreach (var queue in queues)
            {
                if (queue.Name != null && !this.IsInQueue(queue.Name))
                {
                    this._queues.Add(
                        new KeyValuePair<string, bool>(
                            queue.Name,
                            queue.Name == this.DeadLetterExchange
                        )
                    );
                }
            }
        }
    }

    public async Task RefreshQueues()
    {
        this._queues.Clear();
        await this.LoadQueues();
    }

    public async Task RemoveAllQueues()
    {
        await this.LoadQueues();
        foreach (var queue in this.Queues)
        {
            if (queue.Value)
            {
                await this.Channel.QueueDeleteAsync(queue.Key);
            }
        }
        this.Queues.Clear();
        await this.LoadQueues();
    }

    #endregion // Methods

    #region Properties

    protected abstract string AdminPassword { get; }
    protected abstract string AdminPort { get; }
    protected abstract string AdminUser { get; }
    protected IChannel Channel => this._channel;
    protected IConnection Connection => this._connection;
    protected string DeadLetterExchange => "dlx";
    protected abstract string HostName { get; }
    protected string MessageExchange => $"{this.MessageExchangeType}_exchange";
    protected string MessageExchangeType => ExchangeType.Direct;
    public HashSet<KeyValuePair<string, bool>> Queues => this._queues;

    #endregion // Properties
}
