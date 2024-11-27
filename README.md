# RabbitMQ Example

The following solution is an example application comprising of both a cli based
message `consumer` and an api based message `publisher`.

## Structure

- `common` this project contains the core abstract classes for both the
`consumer` and the `publisher`
- `consumers` this project contains the cli for consuming messages
- `publishers` this project contains the api for publishing messages

## Development

For development reference the applicable information per project.

### Consumers

 The consumer documentation: [`consumer`](consumers/README.md)

### Publishers

 The publisher documentation [`publisher`](publishers/README.md)

## Rabbit MQ instance

For this example the RabbitMQ docker container was used.

The following command can be used to start the RabbitMQ container:

```bash
# latest RabbitMQ 4.0.x
docker run -it --rm --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:4.0-management
```

**References:**

- [RabbitMQ Docker](https://hub.docker.com/_/rabbitmq)
- [RabbitMQ Install](https://www.rabbitmq.com/docs/download)
