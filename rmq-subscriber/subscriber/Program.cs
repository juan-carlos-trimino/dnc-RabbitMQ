/***
By default, Visual Studio comes with only one source of NuGet package (Package source: Microsoft
Visual Studio Offline Packages). As a consequence, NuGet will be unable to find the NuGet
RabbitMQ.Client Package. To fix this problem, add one more NuGet package to the "Package source"
with the below values.
Name: nuget.org
Source: https://api.nuget.org/v3/index.json

To add the package, go to
(1) Project > Manage NuGet Packages...
(2) To the right of the "Package source", click the "Settings" icon.
(3) Add the new NuGet package.
***/

// See https://aka.ms/new-console-template for more information
// https://www.rabbitmq.com/dotnet.html
// Failed to create CoreCLR, HRESULT: 0x8007000E

//using System;
using System.Text;
using Newtonsoft.Json;
using Messages;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Newtonsoft.Json.Linq;

//var connection = null;
//var channel = null;

try
{
    string? SVC_DNS = Environment.GetEnvironmentVariable("SVC_DNS");
    if (string.IsNullOrEmpty(SVC_DNS))
    {
        Console.WriteLine("The environment variable SVC_DNS is missing.");
        return;
    }
    Console.WriteLine($"Using the environment variable SVC_DNS: {SVC_DNS}");

    var factory = new ConnectionFactory { Uri = new Uri(SVC_DNS) };
    using var connection = factory.CreateConnection();
    Console.WriteLine("Connection created...");
    using var channel = connection.CreateModel();

    channel.QueueDeclare(queue: "bLoyal",
                         durable: false,
                         exclusive: false,
                         autoDelete: false,
                         arguments: null);
    Console.WriteLine("Queue created...");
    Console.WriteLine(" [*] Waiting for messages.");

    var consumer = new EventingBasicConsumer(channel);
    string? json = null;
    byte[]? body = null;
    Message1 msg1;
    Message2 msg2;
    consumer.Received += (model, ea) =>
    {
        Console.WriteLine(" [x] Received message...");
        body = ea.Body.ToArray();
        json = Encoding.UTF8.GetString(body);
        JObject jobj = JObject.Parse(json);
        int id = Convert.ToInt32(jobj["Id"]);
        if (id % 2 == 0)
        {
            msg1 = JsonConvert.DeserializeObject<Message1>(json);
            Console.WriteLine($"Id = {msg1.Id}");
            Console.WriteLine($"datetime = {msg1.dt}");
            Console.WriteLine($"name = {msg1.name}");
            foreach (string s in msg1.courses)
            {
                Console.WriteLine(s);
            }
        }
        else
        {
            msg2 = JsonConvert.DeserializeObject<Message2>(json) ?? null;
            Console.WriteLine($"Id = {msg2.Id}");
            Console.WriteLine($"datetime = {msg2.dt}");
            Console.WriteLine($"First name = {msg2.firstName}");
            Console.WriteLine($"Middle name = {msg2.middleName}");
            Console.WriteLine($"Last name = {msg2.lastName}");
        }
        Console.WriteLine();
    };
    for (; ; )
    {
        channel.BasicConsume(queue: "bLoyal",
                             autoAck: true,
                             consumer: consumer);
    }
//    Console.WriteLine(" Press [enter] to exit.");
//    Console.ReadLine();
}
finally
{

}