using System;
using System.Threading;
using Zenoh;

Zenoh.Zenoh.InitLogger();
Config config = new Config();
//string[] connect = {"tcp/172.30.100.3:7447"};
//config.SetConnect(connect);
Session session = new Session();

Console.WriteLine("Opening session...");
if (session.Open(config))
{
    Thread.Sleep(200);
}
else
{
    Console.WriteLine("Opening session unsuccessful");
    return;
}

KeyExpr key1 = KeyExpr.FromString("/demo/example/zenoh-cs-put1");
KeyExpr key2 = KeyExpr.FromString("/demo/example/zenoh-cs-put2");

for (int i = 0; i < 5; i++)
{
    KeyExpr key = (i % 2) == 0 ? key1 : key2;

    string data = $"Put from csharp {i}!";
    bool r = session.Put(key, data);
    if (r)
    {
        Console.WriteLine($"Putting Data ('{key.Suffix}': '{data}')");
        Thread.Sleep(50);
    }
    else
    {
        Console.WriteLine("Putting Data Fault!");
        break;
    }
}

session.Close();