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
using Zenoh;
using PowerArgs;

class ZNInfo
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

            Console.WriteLine("Openning session..");
            var s = Zenoh.Net.Session.Open(conf);

            var props = s.Info();
            foreach (KeyValuePair<string, string> entry in props)
            {
                Console.WriteLine("{0} : {1}", entry.Key, entry.Value);
            }

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

    [ArgShortcut("m"), ArgDescription("The zenoh session mode (peer by default). Possible values [peer|client].")]
    public string mode { get; set; }

    [ArgShortcut("e"), ArgDescription("Peer locators used to initiate the zenoh session.")]
    public string peer { get; set; }

    [ArgShortcut("l"), ArgDescription("Locators to listen on.")]
    public string listener { get; set; }

    [ArgShortcut("c"), ArgDescription("A configuration file.")]
    public string config { get; set; }

    public Dictionary<string, string> GetConf()
    {
        Dictionary<string, string> conf;
        if (this.config != null)
        {
            conf = Zenoh.Zenoh.ConfigFromFile(this.config);
        }
        else
        {
            conf = new Dictionary<string, string>();
        }

        if (this.mode != null)
        {
            conf["mode"] = this.mode;
        }
        if (this.peer != null)
        {
            conf["peer"] = this.peer;
        }
        if (this.listener != null)
        {
            conf["listener"] = this.listener;
        }
        return conf;
    }
}
