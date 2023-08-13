using System;
using System.Threading;
using Zenoh;

Zenoh.ZenohC.InitLogger();
Config config = new Config();
string[] connect = { "tcp/127.0.0.1:7447" };
config.SetConnect(connect);
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

KeyExpr keyStr = KeyExpr.FromString("demo/example/zenoh-cs-put/string");
string dataStr = "Put from csharp !";
Console.WriteLine(session.PutStr(keyStr, dataStr)
    ? $"Putting data string ('{keyStr.GetStr()}': '{dataStr}')"
    : "Putting data string fault!");

KeyExpr keyJson = KeyExpr.FromString("demo/example/zenoh-cs-put/json");
string dataJson = "{\"value\": \"Put from csharp\"}";
Console.WriteLine(session.PutJson(keyJson, dataJson)
    ? $"Putting data json ('{keyJson.GetStr()}': {dataJson})"
    : "Putting data json fault!");

KeyExpr keyInt = KeyExpr.FromString("demo/example/zenoh-cs-put/int");
Int64 dataInt = 965;
Console.WriteLine(session.PutInt(keyInt, dataInt)
    ? $"Putting data int ('{keyInt.GetStr()}': {dataInt})"
    : "Putting data int fault!");

KeyExpr keyFloat = KeyExpr.FromString("demo/example/zenoh-cs-put/float");
double dataFloat = 99.6;
Console.WriteLine(session.PutFloat(keyFloat, dataFloat)
    ? $"Putting data float ('{keyFloat.GetStr()}': {dataFloat})"
    : "Putting data float fault!");

session.Close();