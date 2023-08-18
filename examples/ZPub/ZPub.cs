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

string key1 = "demo/example/zenoh-cs-pub1";
string key2 = "demo/example/zenoh-cs-pub2";

Publisher publisher1 = new Publisher(key1);
Publisher publisher2 = new Publisher(key2);

var handle1 = session.RegisterPublisher(publisher1);
if (handle1 is null)
{
    Console.WriteLine($"Register Publisher1 fault On '{key1}'");
    return;
}

Console.WriteLine($"Registered Publisher1 On '{key1}'");

var handle2 = session.RegisterPublisher(publisher2);
if (handle2 is null)
{
    Console.WriteLine($"Register Publisher2 fault On '{key2}'");
    return;
}

Console.WriteLine($"Registered Publisher2 On '{key2}'");

for (int i = 0; i < 100; i++)
{
    string value1 = $"[{i * 2}] Pub from C#!";
    session.PubStr(handle1.Value, value1);
    Console.WriteLine($"Publishing Data ('{key1}': '{value1}')..");

    string value2 = $"[{i * 2 + 1}] Pub from C#!";
    session.PubStr(handle2.Value, value2);
    Console.WriteLine($"Publishing Data ('{key2}': '{value2}')..");

    Thread.Sleep(200);
}

session.UnregisterPublisher(handle1.Value);
session.UnregisterPublisher(handle2.Value);

session.Close();