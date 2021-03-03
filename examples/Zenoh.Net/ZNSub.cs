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

class ZNSub
{

    public static void SubscriberCallback(Zenoh.Net.Sample sample)
    {
        Console.WriteLine(">> [Subscription listener] Received ('{0}': '{1}')",
            sample.ResName,
            BitConverter.ToString(sample.Payload));

        // Encoding.UTF8.GetString(sample.Payload));
    }

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

            Console.WriteLine("Declaring Subscriber on '{0}'...", arguments.selector);
            var rkey = Zenoh.Net.ResKey.RName(arguments.selector);
            var subInfo = new Zenoh.Net.SubInfo();
            s.DeclareSubscriber(rkey, subInfo, ZNSub.SubscriberCallback);

            char c = ' ';
            while (c != 'q')
            {
                c = Console.ReadKey().KeyChar;
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

    [ArgShortcut("m"), ArgDefaultValue("peer"), ArgDescription("The zenoh session mode. Possible values [peer|client].")]
    public string mode { get; set; }

    [ArgShortcut("e"), ArgDescription("Peer locators used to initiate the zenoh session.")]
    public string peer { get; set; }

    [ArgShortcut("l"), ArgDescription("Locators to listen on.")]
    public string listener { get; set; }

    [ArgShortcut("c"), ArgDescription("A configuration file.")]
    public string config { get; set; }

    [ArgShortcut("s"), ArgDefaultValue("/demo/example/**"), ArgDescription("The selection of resources to subscribe.")]
    public string selector { get; set; }

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
