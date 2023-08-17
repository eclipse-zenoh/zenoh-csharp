using System;
using System.Collections.Generic;
using System.Linq;
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

string localId = session.LocalId().ToStr();
Session.Id[] routersId = session.RoutersId();
List<string> routersIdStr = new List<string>();
foreach (var id in routersId)
{
    routersIdStr.Add(id.ToStr());
}
// string[] peerPid = session.PeersZid();

Console.WriteLine($"Local ID: {localId}");
// Console.WriteLine($"PeersPID: {String.Join(',', peerPid)}");
Console.WriteLine($"Routers ID: {String.Join(',', routersIdStr)}");

session.Close();