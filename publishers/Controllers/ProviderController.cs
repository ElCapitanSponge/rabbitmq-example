using Microsoft.AspNetCore.Mvc;

namespace RabbitmqExample.Publishers.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProviderController
{
    #region Constructors

    public ProviderController() { }

    #endregion // Constructors

    #region Fields

    private RabbitmqExample.Publishers.Models.Publisher _publisher =
        new RabbitmqExample.Publishers.Models.Publisher();

    #endregion // Fields

    #region Requests

    [HttpPost("PublishMessageSimple")]
    public string PublishMessage(string queueName, string message)
    {
        this._publisher.SendMessage(new List<string> { queueName }, message);
        return "Simple Message Published";
    }

    [HttpGet("LoadQueues")]
    public HashSet<string> LoadQueues()
    {
        this._publisher.RefreshQueues().Wait();
        return this._publisher.Queues.Where(q => !q.Value).Select(q => q.Key).ToHashSet();
    }
    #endregion // Requests
}
