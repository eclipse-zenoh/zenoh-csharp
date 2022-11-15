using System;
using System.Text;
using System.Threading;
using Zenoh;

//Zenoh.Zenoh.InitLogger();
Config config = new Config();
//string[] connect = {"tcp/172.30.100.3:7447"};
//config.SetConnect(connect);
string[] listen = {"tcp/127.0.0.1:7888"};
config.SetListen(listen);
Session session = new Session();

Console.WriteLine("Opening session...");
if (session.Open(config))
{
    // wait
    Thread.Sleep(200);
}
else
{
    Console.WriteLine("Opening session unsuccessful");
    return;
}

void Callback1(Sample sample)
{
    //string key = sample.Key;
    //string value = sample.ValueToString();
    //Console.WriteLine($">> [Subscriber1] Received PUT ('{key}': '{value}')");
    Console.Write(".");
}

void Callback2(Sample sample)
{
    string key = sample.Key;
    string value = sample.ValueToString();
    Console.WriteLine($">> [Subscriber2] Received PUT ('{key}': '{value}')");
}

SubscriberCallback userCallback1 = Callback1;
SubscriberCallback userCallback2 = Callback2;

string key1 = "demo/example/zenoh-cs-put1";
string key2 = "demo/example/zenoh-cs-put2";

Subscriber subscriber1 = new Subscriber(key1, userCallback1);
Subscriber subscriber2 = new Subscriber(key2, userCallback2);

if (session.RegisterSubscriber(subscriber1))
{
    Console.WriteLine($"Registered Subscriber1 On '{key1}'");
}

if (session.RegisterSubscriber(subscriber2))
{
    Console.WriteLine($"Registered Subscriber2 On '{key2}'");
}

Console.WriteLine("Enter 'q' to quit...");
while (true)
{
    var input = Console.ReadKey();
    if (input.Key == ConsoleKey.Q)
    {
        break;
    }
}

session.Close();