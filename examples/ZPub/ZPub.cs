#nullable enable

using System;
using System.Collections.Generic;
using System.Threading;
using CommandLine;
using Zenoh;

namespace ZPub;

class Program
{
    static void Main(string[] args)
    {
        var r = Parser.Default.ParseArguments<ClArgs>(args);
        bool ok = true;
        r.WithNotParsed(e => { ok = false; });
        if (!ok) return;

        ClArgs clArgs = r.Value;
        Config? config = clArgs.ToConfig();
        if (config is null)
            return;

        Console.WriteLine("Opening session...");
        var session = Session.Open(config);
        if (session is null)
        {
            Console.WriteLine("Opening session fault!");
            return;
        }

        Thread.Sleep(200);
        Console.WriteLine("Opening session successful!");

        string key = clArgs.GetKey();
        string value = clArgs.GetValue();

        Publisher publisher = new Publisher(key);

        var handle = session.RegisterPublisher(publisher);
        if (handle is null)
        {
            Console.WriteLine($"Register Publisher1 fault On '{key}'");
            return;
        }

        Console.WriteLine($"Registered Publisher1 On '{key}'");

        for (int i = 0; i < 100; i++)
        {
            string pubValue = $"[{i}] {value}";
            session.PubStr(handle.Value, pubValue);
            Console.WriteLine($"Publishing Data ('{key}': '{pubValue}')..");

            Thread.Sleep(1000);
        }

        session.UnregisterPublisher(handle.Value);

        session.Close();
    }
}

class ClArgs
{
    [Option('c', "config", Required = false, HelpText = "A configuration file.")]
    public string? ConfigFilePath { get; set; } = null;

    [Option('e', "connect", Required = false, HelpText = "Endpoints to connect to. example: tcp/127.0.0.1:7447")]
    public IEnumerable<string> Connects { get; set; } = new List<string>();

    [Option('l', "listen", Required = false, HelpText = "Endpoints to listen on. example: tcp/127.0.0.1:8447")]
    public IEnumerable<string> Listens { get; set; } = new List<string>();

    [Option('m', "mode", Required = false,
        HelpText = "The zenoh session mode (peer by default) [possible values: peer, client]")]
    public string Mode { get; set; } = "peer";

    [Option('k', "key", Required = false,
        HelpText = "The key expression to publish onto. [default: demo/example/zenoh-cs-pub]")]
    public string? Keyexpr { get; set; } = null;

    [Option('v', "value", Required = false,
        HelpText = "The value to publish. [default: \"Pub from C#!\"]")]
    public string? Value { get; set; } = null;

    internal Config? ToConfig()
    {
        if (ConfigFilePath != null)
        {
            Config? c = Config.LoadFromFile(ConfigFilePath);
            if (c is null)
            {
                Console.WriteLine("load config file error!");
                return null;
            }

            return c;
        }

        Config config = new Config();

        config.SetMode(Mode == "client" ? Config.Mode.Client : Config.Mode.Peer);

        List<string> connects = new List<string>();
        foreach (string s in Connects)
        {
            connects.Add(s);
        }

        config.SetConnect(connects.ToArray());

        List<string> listens = new List<string>();
        foreach (string s in Listens)
        {
            listens.Add(s);
        }

        config.SetConnect(listens.ToArray());

        return config;
    }

    public string GetKey()
    {
        return Keyexpr ?? "demo/example/zenoh-cs-pub";
    }

    public string GetValue()
    {
        return Value ?? "Pub from C#!";
    }
}