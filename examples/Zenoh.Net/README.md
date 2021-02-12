# Zenoh-net C# examples

## Start instructions

   ```bash
   dotnet run -p <example.csproj>
   ```

   Each example accepts the `-h` or `--help` option that provides a description of its arguments and their default values.

   :warning: _To pass any options to an example, specify them after the `--`argument. For instance:_

      ```bash
      dotnet run -p ZNInfo.csproj -- -h
      ```

   If you run the tests against the zenoh router running in a Docker container, you need to add the
   `-e tcp/localhost:7447` option to your examples. That's because Docker doesn't support UDP multicast
   transport, and therefore the zenoh scouting and discrovery mechanism cannot work with.

## Examples description

<!-- ### ZNScout

   Scouts for zenoh peers and routers available on the network.

   Typical usage:
   ```bash
      dotnet run -p ZNScout.csproj
   ``` -->

### ZNInfo

   Gets information about the zenoh-net session.

   Typical usage:
   ```bash
      dotnet run -p ZNInfo.csproj
   ```


### ZNWrite

   Writes a path/value into Zenoh.  
   The path/value will be received by all matching subscribers, for instance the [ZNSub](#ZNSub)
   and [ZNStorage](#ZNStorage) examples.

   Typical usage:
   ```bash
      dotnet run -p ZNWrite.csproj
   ```
   or
   ```bash
      dotnet run -p ZNWrite.csproj -- -p /demo/example/test -v 'Hello World'
   ```

<!-- ### ZNPub

   Declares a resource with a path and a publisher on this resource. Then writes a value using the numerical resource id.
   The path/value will be received by all matching subscribers, for instance the [ZNSub](#ZNSub)
   and [ZNStorage](#ZNStorage) examples.

   Typical usage:
   ```bash
      dotnet run -p ZNPub.csproj
   ```
   or
   ```bash
      dotnet run -p ZNPub.csproj -- -p /demo/example/test -v 'Hello World'
   ``` -->

### ZNSub

   Registers a subscriber with a selector.  
   The subscriber will be notified of each write made on any path matching the selector,
   and will print this notification.

   Typical usage:
   ```bash
      dotnet run -p ZNSub.csproj
   ```
   or
   ```bash
      dotnet run -p ZNSub.csproj -- -s /demo/**
   ```

<!-- ### ZNPull

   Registers a pull subscriber with a selector.  
   The pull subscriber will receive each write made on any path matching the selector,
   and will pull on demand and print the received path/value.

   Typical usage:
   ```bash
      dotnet run -p ZNPull.csproj
   ```
   or
   ```bash
      dotnet run -p ZNPull.csproj -- -s /demo/**
   ``` -->

<!-- ### ZNQuery

   Sends a query message for a selector.  
   The queryables with a matching path or selector (for instance [ZNEval](#ZNEval) and [ZNStorage](#ZNStorage))
   will receive this query and reply with paths/values that will be received by the query callback.

   Typical usage:
   ```bash
      dotnet run -p ZNQuery.csproj
   ```
   or
   ```bash
      dotnet run -p ZNQuery.csproj -- -s /demo/**
   ``` -->

<!-- ### ZNEval

   Registers a queryable function with a path.  
   This queryable function will be triggered by each call to a query operation on zenoh-net
   with a selector that matches the path, and will return a value to the querier.

   Typical usage:
   ```bash
      dotnet run -p ZNEval.csproj
   ```
   or
   ```bash
      dotnet run -p ZNEval.csproj -- -p /demo/example/eval -v 'This is the result'
   ``` -->

<!-- ### ZNStorage

   Trivial implementation of a storage in memory.  
   This examples registers a subscriber and a queryable on the same selector.
   The subscriber callback will store the received paths/values in an hashmap.
   The queryable callback will answer to queries with the paths/values stored in the hashmap
   and that match the queried selector.

   Typical usage:
   ```bash
      dotnet run -p ZNStorage.csproj
   ```
   or
   ```bash
      dotnet run -p ZNStorage.csproj -- -s /demo/**
   ``` -->

<!-- ### ZNPubThr & ZNSubThr

   Pub/Sub throughput test.
   This example allows to perform throughput measurements between a pubisher performing
   write operations and a subscriber receiving notifications of those writes.

   Typical Subscriber usage:
   ```bash
      dotnet run -p ZNSubThr.csproj
   ```

   Typical Publisher usage:
   ```bash
      dotnet run -p ZNPubThr.csproj -- 1024
   ``` -->