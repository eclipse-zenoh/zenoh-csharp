using System;
using System.Linq;
using System.Threading;
using Zenoh;

Zenoh.Zenoh.InitLogger();
Config config = new Config();
config.SetId("01");
//string[] c = { "tcp/172.30.100.3:7447", "tcp/172.30.100.1:7447" };
//config.SetConnect(c);
config.SetMode(Config.Mode.Client);
config.SetTimestamp(true);
string configStr = config.ToStr();
Console.WriteLine($"config string:\n{configStr}");

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


Info info = session.Info();
string pid = info.Pid();
string[] routerPid = info.RouterPid();
string[] peerPid = info.PeerPid();
info.Dispose();

Console.WriteLine($"PID: {pid}\nPeer PID: {String.Join(',', peerPid)}\nRouter PID: {String.Join(',', routerPid)}");

session.Close();