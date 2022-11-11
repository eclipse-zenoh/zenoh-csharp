using System;
using System.Linq;
using System.Threading;
using Zenoh;

//Zenoh.Zenoh.InitLogger();
Config config = new Config();
//config.SetId("01");
//string[] c = { "tcp/172.30.100.3:7447", "tcp/172.30.100.1:7447" };
//config.SetConnect(c);
config.SetMode(Config.Mode.Client);
config.SetTimestamp(true);
string configStr = config.ToStr();
Console.WriteLine($"config string:\n{configStr}\n----------------------\n");

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

string pid = session.LocalZid();
string[] routerPid = session.RoutersZid();
string[] peerPid = session.PeersZid();

Console.WriteLine($"Local PID: {pid}");
Console.WriteLine($"PeersPID: {String.Join(',', peerPid)}");
Console.WriteLine($"Routers PID: {String.Join(',', routerPid)}");

session.Close();