#nullable enable

using System;
using System.Threading;
using Zenoh;

Config config = new Config();
config.SetMode(Config.Mode.Client);
string[] connect = { "tcp/127.0.0.1:7447" };
config.SetConnect(connect);
//string[] listen = {"tcp/127.0.0.1:7888"};
//config.SetListen(listen);

Console.WriteLine("Opening session...");
var session = Session.Open(config);
if (session is null)
{
    Console.WriteLine("Opening session fault!");
    return;
}

Thread.Sleep(200);
Console.WriteLine("Opening session successful!");


void Callback1(Sample sample)
{
    string key = sample.GetKeyexpr();
    string value = sample.GetString() ?? "";
    Console.WriteLine($">> [Subscriber1] Received PUT ('{key}': '{value}')");
}

void Callback2(Sample sample)
{
    string key = sample.GetKeyexpr();
    string value = sample.GetString() ?? "";
    Console.WriteLine($">> [Subscriber2] Received PUT ('{key}': '{value}')");
}

SubscriberCallback userCallback1 = Callback1;
SubscriberCallback userCallback2 = Callback2;

string key1 = "demo/example/zenoh-cs-put1";
string key2 = "demo/example/zenoh-cs-put2";

Subscriber subscriber1 = new Subscriber(key1, userCallback1);
Subscriber subscriber2 = new Subscriber(key2, userCallback2);

var handle1 = session.RegisterSubscriber(subscriber1);
if (handle1 is null)
{
    Console.WriteLine($"Register Subscriber1 fault On '{key1}'");
    return;
}

Console.WriteLine($"Registered Subscriber1 On '{key1}'");

var handle2 = session.RegisterSubscriber(subscriber2);
if (handle2 is null)
{
    Console.WriteLine($"Register Subscriber2 fault On '{key2}'");
    return;
}

Console.WriteLine($"Registered Subscriber2 On '{key2}'");

Console.WriteLine("Enter 'q' to quit...");
while (true)
{
    var input = Console.ReadKey();
    if (input.Key == ConsoleKey.Q)
    {
        break;
    }
}

session.UnregisterSubscriber((SubscriberHandle)handle1);
session.UnregisterSubscriber((SubscriberHandle)handle2);

session.Close();