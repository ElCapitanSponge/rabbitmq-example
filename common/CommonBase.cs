using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using RabbitMQ.Client;

namespace RabbitmqExample.Common;

public interface ICommonBase { }

public abstract class CommonBase : ICommonBase
{
    #region Constructors

    public CommonBase()
    {
        var factory = new ConnectionFactory { HostName = this.HostName };
        this._connection = factory.CreateConnectionAsync().Result;
        this._channel = this.Connection.CreateChannelAsync().Result;
        this.LoadQueues().Wait();
    }

    #endregion // Constructors

    #region Fields

    private IChannel _channel;
    private IConnection _connection;
    private readonly HashSet<string> _queues = new HashSet<string>();

    #endregion // Fields

    #region Methods

    protected void DeclareQueueIfNotDeclared(string queueName)
    {
        if (!this.Queues.Contains(queueName))
        {
            this.Channel.QueueDeclareAsync(
                queue: queueName,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null
            ).Wait();
            this.Queues.Add(queueName);
        }
    }

    public void Dispose()
    {
        this.Channel?.CloseAsync();
        this.Connection?.CloseAsync();
    }

    private async Task LoadQueues()
    {
        var httpClient = new HttpClient();
        var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes("guest:guest"));
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Basic",
            credentials
        );

        var queues = await httpClient.GetFromJsonAsync<List<QueueInfo>>(
            $"http://{this.HostName}:15672/api/queues"
        );
        if (queues != null)
        {
            foreach (var queue in queues)
            {
                this._queues.Add(queue.Name);
            }
        }
    }

    #endregion // Methods

    #region Properties

    protected IChannel Channel => this._channel;
    protected IConnection Connection => this._connection;
    protected abstract string HostName { get; }
    protected HashSet<string> Queues => this._queues;

    #endregion // Properties
}

public interface IQueueInfo
{
    public string Name { get; set; }
}

public class QueueInfo : IQueueInfo
{
    #region Properties

    public string Name { get; set; }

    #endregion // Properties
}
