using System;
using System.Threading;
using Zenoh;

//Zenoh.Zenoh.InitLogger();
Config config = new Config();
//string[] connect = {"tcp/172.30.100.3:7447"};
//config.SetConnect(connect);
Session session = new Session();

Console.WriteLine("Opening session...");
if (session.Open(config))
{
    // wait
    Thread.Sleep(1000);
}
else
{
    Console.WriteLine("Opening session unsuccessful");
    return;
}

KeyExpr key1 = KeyExpr.FromString("/demo/example/zenoh-cs-put1");
KeyExpr key2 = KeyExpr.FromString("/demo/example/zenoh-cs-put2");

Console.WriteLine($"Sending Query '{key1.GetStr()}'");
ReplayDataArray data1 = session.Get(key1);
foreach (ReplayData ele in data1.List)
{
    string s = $">> Received ('{ele.Sample.Key}': '{ele.Sample.ValueToString()}'";
    Console.WriteLine(s);
}


ReplayDataArray data2 = session.Get(key2);
Console.WriteLine($"Sending Query '{key2.GetStr()}'");
foreach (ReplayData ele in data2.List)
{
    string s = $">> Received ('{ele.Sample.Key}': '{ele.Sample.ValueToString()}'";
    Console.WriteLine(s);
}

session.Close();