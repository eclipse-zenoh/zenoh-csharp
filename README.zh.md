![zenoh banner](./zenoh-dragon.png)

[![NuGet](https://img.shields.io/nuget/v/Zenoh-CS?color=blue)](https://www.nuget.org/packages/Zenoh-CS/)
[![License](https://img.shields.io/badge/License-EPL%202.0-blue)](https://choosealicense.com/licenses/epl-2.0/)
[![License](https://img.shields.io/badge/License-Apache%202.0-blue.svg)](https://opensource.org/licenses/Apache-2.0)

# Zenoh C# API

[Zenoh](http://zenoh.io)是一种非常高效和容错的命名数据网络([NDN](http://named-data.net))协议, 能够在非常受限制的设备和网络中运行.

C# API是用于纯客户端的, 可以很容易地针对运行在Docker容器中的zenoh路由器进行测试 (参考[快速测试](https://zenoh.io/docs/getting-started/quick-test/)).

-------------------------------
## 如何安装

需求:
- 库 [zenoh-c](https://github.com/eclipse-zenoh/zenoh-c) 必需被安装在你的主机上.
- Zenoh C# 库 [Zenoh-CS](https://www.nuget.org/packages/Zenoh-CS/) 库在NuGet上可用(只支持x64)

### 支持的 .NET 标准 
- .NET 6.0
- .NET 7.0

### 支持的CPU架构
- x64
- arm64 (未测试)

### Zenoh-CS 版本与 Zenoh-C 版本对应关系
|  Zenoh-C  | Zenoh-CS |
|:---------:|:--------:|
| v0.7.2-rc |  v0.1.*  |


-------------------------------
## 如何构建 

需求:
- 库 [zenoh-c](https://github.com/eclipse-zenoh/zenoh-c) 必需被安装在你的主机上.
- 主机安装有 .NET6 或 .NET7 的 [SDK](https://dotnet.microsoft.com/zh-cn/download/dotnet)

构建命令:   
由于Zenoh-C的部分数据结构在 `x64` 与 `arm64` 下长度不一样, 所以构建时需要增加选项 `-p:Platform=x64` 或 `-p:Platform=ARM64`
```shell
# x64 CPU
dotnet build Zenoh.csproj -c Release -p:Platform=x64
# arm64 CPU
dotnet build Zenoh.csproj -c Release -p:Platform=ARM64
```


-------------------------------
## 运行示例

构建和运行示程序, 参考  [examples/README.zh.md](https://github.com/sanri/zenoh-csharp/blob/master/examples/README.zh.md)

