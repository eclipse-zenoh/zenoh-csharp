#nullable enable

using System;
using System.Threading;
using Zenoh;

 Config config = new Config();
 if (!config.SetMode(Config.Mode.Client))
 {
     Console.WriteLine("Config set mode fail");
     return;
 }
 string[] connect = { "tcp/127.0.0.1:7447" };
 if (!config.SetConnect(connect))
 {
     Console.WriteLine("Config set connect fail");
     return;
 }
 
// string[] listen = {"tcp/127.0.0.1:7888"};
// config.SetListen(listen);

// Config? c = Config.LoadFromFile("../../../../zenoh.json5");
// if (c == null)
// {
//     Console.WriteLine("Load config error!");
//     return;
// }
// Config config = c;

Console.WriteLine("Opening session...");
var session = Session.Open(config);
if (session is null)
{
    Console.WriteLine("Opening session unsuccessful!");
    return;
}

Thread.Sleep(200);
Console.WriteLine("Opening session successful!");

string keyStr = "demo/example/zenoh-cs-put/string";
string dataStr = "Put from csharp !";
Console.WriteLine(session.PutStr(keyStr, dataStr)
    ? $"Putting data string ('{keyStr}': '{dataStr}')"
    : "Putting data string fault!");

string keyJson = "demo/example/zenoh-cs-put/json";
string dataJson = "{\"value\": \"Put from csharp\"}";
Console.WriteLine(session.PutJson(keyJson, dataJson)
    ? $"Putting data json ('{keyJson}': {dataJson})"
    : "Putting data json fault!");

string keyInt = "demo/example/zenoh-cs-put/int";
long dataInt = 965;
Console.WriteLine(session.PutInt(keyInt, dataInt)
    ? $"Putting data int ('{keyInt}': {dataInt})"
    : "Putting data int fault!");

string keyFloat = "demo/example/zenoh-cs-put/float";
double dataFloat = 99.6;
Console.WriteLine(session.PutFloat(keyFloat, dataFloat)
    ? $"Putting data float ('{keyFloat}': {dataFloat})"
    : "Putting data float fault!");

session.Close();