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

    private RabbitmqExample.Publishers.Models.Publisher? _publisher;

    #endregion // Fields

    #region Methods

    private void CheckPublisherInitialised(bool isSetQueue = false)
    {
        if (!this.IsPublisherInitialised && !isSetQueue)
        {
            throw new InvalidOperationException(
                "Publisher is not initialised. Please Specify the Queue Names"
            );
        }
    }

    #endregion // Methods

    #region Properties

    private RabbitmqExample.Publishers.Models.Publisher? Publisher
    {
        get => this._publisher;
        set => this._publisher = value;
    }

    private bool IsPublisherInitialised => this._publisher != null;

    #endregion // Properties

    #region Requests

	[HttpPost("PublishMessage")]
	public void PublishMessage([FromBody] string queueName, [FromBody] string message)
	{
		this.CheckPublisherInitialised();
		this.Publisher?.PublishMessage(queueName, message);
	}

	[HttpPost("SetQueues")]
	public List<string> SetQueues([FromBody] List<string> queueNames)
	{
		this.Publisher = new Publishers.Models.Publisher(queueNames);
		return this.Publisher.AvailiableQueues;
	}

    #endregion // Requests
}
