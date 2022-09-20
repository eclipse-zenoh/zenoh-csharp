# Zenoh C# examples

## Start instructions

   ```bash
   dotnet run -p <example.csproj>
   ```

   Each example accepts the `-h` or `--help` option that provides a description of its arguments and their default values.

   :warning: _To pass any options to an example, specify them after the `--`argument. For instance:_

      ```bash
      dotnet run -p ZGet.csproj -- -h
      ```

   If you run the tests against the zenoh router running in a Docker container, you need to add the
   `-e tcp/localhost:7447` option to your examples. That's because Docker doesn't support UDP multicast
   transport, and therefore the zenoh scouting and discrovery mechanism cannot work with.

## Examples description


### ZGet

   Gets value from zenoh session.

   Typical usage:
   ```bash
      dotnet run -p ZGet.csproj
   ```


### ZPut

   Writes a path/value into Zenoh.  
   The path/value will be received by all matching subscribers, for instance the [ZSub](#ZSub) examples.

   Typical usage:
   ```bash
      dotnet run -p ZPut.csproj
   ```

### ZSub

   Registers a subscriber with a selector.  
   The subscriber will be notified of each write made on any path matching the selector,
   and will print this notification.

   Typical usage:
   ```bash
      dotnet run -p ZSub.csproj
   ```
