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
using RabbitMQ.Client;
using Messages;

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

    //.ToString("yyyyMMddHHmmss.fff")
    string? message = null;
    byte[]? body = null;
    var counter = 0;
    var max = 1_000; //args.Length is not 0 ? Convert.ToInt32(args[0]) : -1;
    Message1 msg1 = new Message1();
    Message2 msg2 = new Message2();
    while (counter < max)
    {
        if (counter % 2 == 0)
        {
            msg1.Id = counter;
            msg1.dt = DateTime.Now;
            msg1.name = $"Alex Smith{counter}";
            msg1.courses = new List<string>()
            {
                "Math230",
                "Calculus 1",
                "CS2001",
                "ML"
            };
            body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(msg1));
        }
        else
        {
            msg2.Id = counter;
            msg2.dt = DateTime.Now;
            msg2.firstName = $"Alex{counter}";
            msg2.middleName = "Alex";
            msg2.lastName = "Smith";
            body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(msg2));
        }
        channel.BasicPublish(exchange: string.Empty,
                             routingKey: "bLoyal",
                             basicProperties: null,
                             body: body);
        Console.WriteLine($" [x] Sent {Encoding.UTF8.GetString(body)}");
        await Task.Delay(TimeSpan.FromMilliseconds(1_000));
        ++counter;
    }

    Console.WriteLine(" Press [enter] to exit.");
    Console.ReadLine();
}
finally
{

}