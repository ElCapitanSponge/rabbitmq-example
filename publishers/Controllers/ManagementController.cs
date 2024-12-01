using Microsoft.AspNetCore.Mvc;

namespace RabbitmqExample.Publishers.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ManagementController
{
    #region Constructors

    public ManagementController() { }

    #endregion // Constructors

    #region Fields

    private RabbitmqExample.Publishers.Models.Management _management =
        new RabbitmqExample.Publishers.Models.Management();

    #endregion // Fields

    #region Requests

    [HttpPut("CreateQueue")]
    public Task<bool> CreateQueue(string queueName)
    {
        return this._management.CreateQueue(queueName);
    }

    [HttpPut("CreateQueues")]
    public async Task<List<KeyValuePair<string, bool>>> CreateQueues(List<string> queueNames)
    {
        List<KeyValuePair<string, bool>> results = new List<KeyValuePair<string, bool>>();
        foreach (string queueName in queueNames)
        {
            bool hidden = await this._management.CreateQueue(queueName);
            results.Add(
                new KeyValuePair<string, bool>(queueName, hidden)
            );
        }
        return results;
    }

    [HttpGet("LoadQueues")]
    public HashSet<string> LoadQueues()
    {
        this._management.RefreshQueues().Wait();
        return this._management.Queues.Where(q => !q.Value).Select(q => q.Key).ToHashSet();
    }

    [HttpDelete("DeleteAllQueues")]
    public Task DeleteAllQueues()
    {
        this._management.RemoveAllQueues().Wait();
        return Task.CompletedTask;
    }

    #endregion // Requests
}
