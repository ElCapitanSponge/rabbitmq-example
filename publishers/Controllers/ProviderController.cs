using System.Text;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;

namespace RabbitmqExample.Publishers.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProviderController
{
	#region Constructors

	public ProviderController() { }

	#endregion // Constructors

	#region Requests

	[HttpPost("PublishMessage")]
	public async Task<string> PublishMessage(string queueName, string message)
	{
		var factory = new ConnectionFactory { HostName = "localhost" };
		using var connection = await factory.CreateConnectionAsync();
		using var channel = await connection.CreateChannelAsync();

		await channel.QueueDeclareAsync(queue: queueName, durable: false, exclusive: false, autoDelete: false,
			arguments: null);

		var body = Encoding.UTF8.GetBytes(message);

		await channel.BasicPublishAsync(exchange: string.Empty, routingKey: queueName, body: body);
		return $" [{queueName}] Sent {message}";
	}

	#endregion // Requests
}
