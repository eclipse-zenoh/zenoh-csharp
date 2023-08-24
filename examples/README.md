# Zenoh C# examples

## Introduce 

Each example accepts the `--help` option that provides a description of its arguments and their default values.

If you run the tests against the zenoh router running in a Docker container, 
you need to add the `-e tcp/localhost:7447` option to your examples. 
That's because Docker doesn't support UDP multicast transport, 
and therefore the zenoh scouting and discrovery mechanism cannot work with.

When building, option `--property:Platform=<>` are required.

## Examples description

### ZInfo

Get the ID of each node in the Zenoh network.

Build
```bash
dotnet build ZInfo/ZInfo.csproj --configuration Release --property:Platform=x64  
```

Run
```bash
./ZInfo 
```

### ZGet

Sends a query message for a selector.
The queryables with a matching path or selector (for instance ZQueryable ) will receive this query and reply with paths/values that will be received by the receiver stream.

Build
```bash
dotnet build ZGet/ZGet.csproj --configuration Release --property:Platform=x64  
```

Run
```bash
./ZGet 
```

### ZQueryable

Declares a queryable function with a path.
This queryable function will be triggered by each call to get with a selector that matches the path, and will return a value to the querier.

Build
```bash
dotnet build ZQueryable/ZQueryable.csproj --configuration Release --property:Platform=x64  
```

Run
```bash
./ZQueryable
```


### ZPut

Writes a path/value into Zenoh.  
The path/value will be received by all matching subscribers, for instance the [ZSub](#ZSub) examples.

Build
```bash
dotnet build ZPut/ZPut.csproj --configuration Release --property:Platform=x64  
```

Run
```bash
./ZPut
```

### ZPub
Writes a path/value into Zenoh.  
The path/value will be received by all matching subscribers, for instance the [ZSub](#ZSub) examples.    
Compared to ZPut, this example is optimized for keys that publish data frequently, reducing some of the overhead.

Build
```bash
dotnet build ZPub/ZPub.csproj --configuration Release --property:Platform=x64  
```

Run
```bash
./ZPub
```


### ZSub

Registers a subscriber with a selector.  
The subscriber will be notified of each write made on any path matching the selector,
and will print this notification.

Build
```bash
dotnet build ZSub/ZSub.csproj --configuration Release --property:Platform=x64  
```

Run
```bash
./ZSub
```