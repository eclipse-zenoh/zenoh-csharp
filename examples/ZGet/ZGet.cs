#nullable enable

using System;
using System.Text;
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

string keyexpr = "demo/**";
byte[] data = Encoding.UTF8.GetBytes("hello");
QueryOptions queryOptions = new QueryOptions(keyexpr, EncodingPrefix.TextPlain, data);

Console.WriteLine($"Sending Query '{keyexpr}'");
Querier? querier = session.Query(queryOptions);
if (querier is null)
{
    Console.WriteLine("Session query fault!");
    goto EXIT;
}

QuerierCallback callback = sample =>
{
    string key = sample.GetKeyexpr();
    EncodingPrefix encodingPrefix = sample.GetEncodingPrefix();
    string? s = sample.GetString();
    if (s is null)
    {
        byte[] d = sample.GetPayload();
        Console.WriteLine($">> Received ('{key}' '{encodingPrefix}': '{d}')");
    }
    else
    {
        Console.WriteLine($">> Received ('{key}' '{encodingPrefix}': '{s}')");
    }
};

if (!querier.GetSamples(callback))
{
    Console.WriteLine("querier get sample error!");
}

EXIT:
session.Close();