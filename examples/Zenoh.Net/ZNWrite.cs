//
// Copyright (c) 2021 ADLINK Technology Inc.
//
// This program and the accompanying materials are made available under the
// terms of the Eclipse Public License 2.0 which is available at
// http://www.eclipse.org/legal/epl-2.0, or the Apache License, Version 2.0
// which is available at https://www.apache.org/licenses/LICENSE-2.0.
//
// SPDX-License-Identifier: EPL-2.0 OR Apache-2.0
//
// Contributors:
//   ADLINK zenoh team, <zenoh@adlink-labs.tech>
//
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;
using Zenoh;
using PowerArgs;

class ZNWrite
{
    static void Main(string[] args)
    {
        try
        {
            // initiate logging
            Zenoh.Zenoh.InitLogger();

            // arguments parsing
            var arguments = Args.Parse<ExampleArgs>(args);
            if (arguments == null) return;
            Dictionary<string, string> conf = arguments.GetConf();

            Console.WriteLine("Openning session...");
            var s = Zenoh.Net.Session.Open(conf);

            var rkey = Zenoh.Net.ResKey.RName(arguments.path);

            Console.WriteLine("Writing Data ({0}, {1})...", rkey, arguments.value);
            s.Write(rkey, Encoding.UTF8.GetBytes(arguments.value));

            s.Dispose();
        }
        catch (ArgException)
        {
            Console.WriteLine(ArgUsage.GenerateUsageFromTemplate<ExampleArgs>());
        }
    }
}


[ArgExceptionBehavior(ArgExceptionPolicy.StandardExceptionHandling)]
public class ExampleArgs
{
    [HelpHook, ArgShortcut("h"), ArgDescription("Shows this help")]
    public Boolean help { get; set; }

    [ArgShortcut("m"), ArgDefaultValue("peer"), ArgDescription("The zenoh session mode. Possible values [peer|client].")]
    public string mode { get; set; }

    [ArgShortcut("e"), ArgDescription("Peer locators used to initiate the zenoh session.")]
    public string peer { get; set; }

    [ArgShortcut("l"), ArgDescription("Locators to listen on.")]
    public string listener { get; set; }

    [ArgShortcut("c"), ArgDescription("A configuration file.")]
    public string config { get; set; }

    [ArgShortcut("p"), ArgDefaultValue("/demo/example/zenoh-csharp-write"), ArgDescription("The name of the resource to write.")]
    public string path { get; set; }

    [ArgShortcut("v"), ArgDefaultValue("Write from C#!"), ArgDescription("The value of the resource to write.")]
    public string value { get; set; }

    public Dictionary<string, string> GetConf()
    {
        var conf = new Dictionary<string, string>();
        conf.Add("mode", this.mode);
        if (this.peer != null)
        {
            conf.Add("peer", this.peer);
        }
        if (this.listener != null)
        {
            conf.Add("listener", this.listener);
        }
        if (this.config != null)
        {
            conf.Add("config", this.config);
        }
        return conf;
    }
}
