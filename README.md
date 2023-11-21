![zenoh banner](./zenoh-dragon.png)

[![NuGet](https://img.shields.io/nuget/v/Zenoh-CS?color=blue)](https://www.nuget.org/packages/Zenoh-CS/)
[![License](https://img.shields.io/badge/License-EPL%202.0-blue)](https://choosealicense.com/licenses/epl-2.0/)
[![License](https://img.shields.io/badge/License-Apache%202.0-blue.svg)](https://opensource.org/licenses/Apache-2.0)

# Zenoh C# API

[中文/chinese readme](https://github.com/sanri/zenoh-csharp/blob/master/README.zh.md)

[Zenoh](http://zenoh.io) is an extremely efficient and fault-tolerant [Named Data Networking](http://named-data.net) (NDN) protocol that is able to scale down to extremely constrainded devices and networks.

The C# API is for pure clients, in other terms does not support peer-to-peer communication, 
can be easily tested against a zenoh router running in a Docker container (see https://zenoh.io/docs/getting-started/quick-test/).


-------------------------------
## How to install it

Requirements:
- The [zenoh-c](https://github.com/eclipse-zenoh/zenoh-c) library must be installed on your host
- The zenoh C# library [Zenoh-CS](https://www.nuget.org/packages/Zenoh-CS/) is available on NuGet(Only x64).

### Supported .NET Standards
- .NET 6.0
- .NET 7.0

### Supported CPU arch
- x64
- arm64 (untested)

### Mapping between Zenoh-CS and Zenoh-C versions
|  Zenoh-C  | Zenoh-CS |
|:---------:|:--------:|
| v0.7.2-rc |  v0.1.*  |


-------------------------------
## How to build it

Requirements:  
 * The [zenoh-c](https://github.com/eclipse-zenoh/zenoh-c) library must be installed on your host
 * A .NET environment. .NET6 or .NET7 [SDK](https://dotnet.microsoft.com/zh-cn/download/dotnet)

Build:   
Because some of Zenoh-C data structures are not the same length in `x64` and `arm64`, you need to add the option `-p:Platform=x64` or `-p:Platform=ARM64` when building.
```shell
# x64 CPU
dotnet build Zenoh.csproj -c Release -p:Platform=x64
# arm64 CPU
dotnet build Zenoh.csproj -c Release -p:Platform=ARM64
```

-------------------------------
## Running the Examples

Build and run the zenoh-csharp examples following the instructions in [examples/README.md](https://github.com/sanri/zenoh-csharp/blob/master/examples/README.md)
