using System;
using System.Threading;
using CommandLine;
using Zenoh;

namespace ZPut;

public class Program
{
    static void Main(string[] args)
    {
        var arguments = Parser.Default.ParseArguments<Args>(args);
        var isOk = true;
        arguments.WithNotParsed(e => { isOk = false; });
        if (!isOk) return;

        var clArgs = arguments.Value;
        var config = clArgs.ToConfig();
        if (config is null) return;

        // Console.WriteLine("Opening session...");
        // var r = Session.Open(config, out var session);
        // if (session is null)
        // {
        //     Console.WriteLine($"Opening session unsuccessful! result: {r}");
        //     return;
        // }
        //
        // Console.WriteLine("Opening session successful!");
        // Thread.Sleep(2000);

        Keyexpr? keyexpr;
        ZBytes payload;
        PutOptions options;
        // string printString;
        

        // ----------------------------------------------------------------------
        string keyStr = "demo/example/zenoh-cs-put/string";
        string dataStr = "Put from csharp !";
        
        keyexpr = Keyexpr.FromString(keyStr);
        Console.WriteLine("1");
        if (keyexpr is null) goto Exit;
        
        Console.WriteLine("2");
        // payload = ZBytes.FromString(dataStr);
        // Console.WriteLine("3");
        
        // options = new PutOptions(EncodingId.TextPlain);
        // Console.WriteLine("4");
        
        // r = session.Put(keyexpr, payload, options);
        // Console.WriteLine("4");
        //
        // printString = r != Result.Ok
        //     ? $"Putting data failed! result: {r}, key: {keyStr}"
        //     : $"Putting data succeeded! key: {keyStr}, payload: {dataStr}";
        // Console.WriteLine(printString);


        // ----------------------------------------------------------------------
        // string keyJson = "demo/example/zenoh-cs-put/json";
        // string dataJson = "{\"value\": \"Put from csharp\"}";
        // keyexpr = Keyexpr.FromString(keyJson);
        // if (keyexpr is null) goto Exit;
        // payload = ZBytes.FromString(dataJson);
        // options = new PutOptions(EncodingId.TextJson);
        //
        // r = session.Put(keyexpr, payload, options);
        //
        // printString = r != Result.Ok
        //     ? $"Putting data failed! result: {r}, key: {keyJson}"
        //     : $"Putting data succeeded! key: {keyJson}, payload: {dataJson}";
        // Console.WriteLine(printString);

        
        
        // string keyInt = "demo/example/zenoh-cs-put/int";
        // long dataInt = 965;
        // Console.WriteLine(session.PutInt(keyInt, dataInt)
        //     ? $"Putting data int ('{keyInt}': {dataInt})"
        //     : "Putting data int fault!");
        //
        // string keyFloat = "demo/example/zenoh-cs-put/float";
        // double dataFloat = 99.6;
        // Console.WriteLine(session.PutFloat(keyFloat, dataFloat)
        //     ? $"Putting data float ('{keyFloat}': {dataFloat})"
        //     : "Putting data float fault!");
        //
        // string keyBin = "demo/example/zenoh-cs-put/bin";
        // byte[] dataBin = { 0x1, 0x2, 0x3, 0x4 };
        // Console.WriteLine(
        //     session.PutData(keyBin, dataBin, ZEncodingPrefix.AppCustom)
        //         ? $"Putting data bin ('{keyBin}': {dataBin.Length} Byte"
        //         : "Putting data bin fault!");

        
        Exit:
        // session.Close();
        Console.WriteLine("exit");
    }
}

public class Args
{
    [Option('c', "config", Required = false, HelpText = "Zenoh config file.")]
    public string? ConfigFilePath { get; set; } = null;
    
    public Config? ToConfig()
    {
        var config = ConfigFilePath == null ? Config.Default() : Config.FromFile(ConfigFilePath);
            
        if (config is not null) return config;
        Console.WriteLine("load config file error!");
        return null;
    }
    
}