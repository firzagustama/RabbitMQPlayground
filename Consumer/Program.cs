﻿using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

// Analytics Consumer
var factory = new ConnectionFactory() { HostName = "localhost" };

using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

channel.ExchangeDeclare(exchange: "topic", type: ExchangeType.Topic);

var queueName = channel.QueueDeclare().QueueName;
channel.QueueBind(queue: queueName, exchange: "topic", routingKey: "*.europe.*");

var consumer = new EventingBasicConsumer(channel);
consumer.Received += (model, ea) =>
{
    var body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    Console.WriteLine($"Analytics - Received new message: {message}");
};

channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);
Console.WriteLine("Analytics Consuming");
Console.ReadKey();