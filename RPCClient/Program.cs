using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;

var factory = new ConnectionFactory() { HostName = "localhost" };
var connection = factory.CreateConnection();
var channel = connection.CreateModel();

var replyQueue = channel.QueueDeclare("", exclusive: true);
channel.QueueDeclare("request-queue", exclusive: false);

var consumer = new EventingBasicConsumer(channel);
consumer.Received += (model, ea) =>
{
    var body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    Console.WriteLine($"Reply Received: {message}");
};

channel.BasicConsume(queue: replyQueue.QueueName, autoAck: true, consumer: consumer);

var properties = channel.CreateBasicProperties();
properties.ReplyTo = replyQueue.QueueName;
properties.CorrelationId = Guid.NewGuid().ToString();

var message = "Can I request a reply?";
var body = Encoding.UTF8.GetBytes(message);

Console.WriteLine($"Sending Request: {properties.CorrelationId}");
channel.BasicPublish("", "request-queue", properties, body);
Console.ReadKey();