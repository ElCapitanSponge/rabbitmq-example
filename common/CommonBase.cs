using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using RabbitMQ.Client;

namespace RabbitmqExample.Common;

public interface ICommonBase
{
    public bool CreateQueue(string queueName);
    public void Dispose();
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
        this.LoadQueues().Wait();
        this.CreateDeadLetterExchange();
    }

    #endregion // Constructors

    #region Fields

    private IChannel _channel;
    private IConnection _connection;
    private readonly HashSet<KeyValuePair<string, bool>> _queues =
        new HashSet<KeyValuePair<string, bool>>();

    #endregion // Fields

    #region Methods

    public bool CreateQueue(string queueName)
    {
        this.DeclareQueueIfNotDeclared(queueName);
        bool exists = this.IsInQueue(queueName);
        return exists;
    }

    protected void DeclareQueueIfNotDeclared(string queueName, bool isDeadLetter = false)
    {
        if (!this.IsInQueue(queueName))
        {
            this.Channel.ExchangeDeclareAsync(
                    exchange: $"exchange-{queueName}",
                    type: ExchangeType.Direct
                )
                .Wait();
            IDictionary<string, object?>? args = null;
            if (!isDeadLetter)
            {
                args = new Dictionary<string, object?>
                {
                    { "x-dead-letter-exchange", $"exchange-{this.DeadLetterExchange}" },
                    { "x-dead-letter-routing-key", $"{this.DeadLetterExchange}-key" },
                };
            }
            this.Channel.QueueDeclareAsync(
                    queue: queueName,
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: args
                )
                .Wait();
            this.Channel.QueueBindAsync(
                    queue: queueName,
                    exchange: $"exchange-{queueName}",
                    routingKey: $"{queueName}-key"
                )
                .Wait();
            this.Queues.Add(new KeyValuePair<string, bool>(queueName, isDeadLetter));
        }
    }

    public void Dispose()
    {
        this.Channel?.CloseAsync();
        this.Connection?.CloseAsync();
    }

    private void CreateDeadLetterExchange()
    {
        if (!this.IsInQueue(this.DeadLetterExchange))
        {
            this.DeclareQueueIfNotDeclared(this.DeadLetterExchange, true);
        }
    }

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
                            !(queue.Name == this.DeadLetterExchange)
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
    public HashSet<KeyValuePair<string, bool>> Queues => this._queues;

    #endregion // Properties
}
