<img src="Nethermind.png" width="200">

# .NET Core Ethereum client
|           |         |               |
| :-------- | :------ | :------------ |
| Documentation | [![ReadtheDocs](https://readthedocs.org/projects/nethermind/badge/?version=latest)](https://nethermind.readthedocs.io) | https://nethermind.readthedocs.io |
| Gitter | [![Gitter](https://img.shields.io/gitter/room/nethermindeth/nethermind.svg)](https://gitter.im/nethermindeth/nethermind) | https://gitter.im/nethermindeth/nethermind |
| Discord | [![Discord](https://img.shields.io/discord/629004402170134531)](https://discord.gg/GXJFaYk) |
| Medium | | https://medium.com/nethermind-eth |
| Twitter | | https://twitter.com/nethermindeth |
| Releases | [![GitHub release](https://img.shields.io/github/release/NethermindEth/nethermind.svg)](https://github.com/NethermindEth/nethermind/releases) | https://github.com/NethermindEth/nethermind/releases |
| Website | | https://nethermind.io/ |
|Docker||https://hub.docker.com/r/nethermind/nethermind|
|Codecov.io| [![codecov](https://codecov.io/gh/NethermindEth/nethermind/branch/master/graph/badge.svg)](https://codecov.io/gh/NethermindEth/nethermind) | https://codecov.io/gh/NethermindEth/nethermind |
| Fund | with Gitcoin | https://gitcoin.co/grants/142/nethermind |
| Github Actions | ![Standard Build](https://github.com/NethermindEth/nethermind/workflows/Standard%20Build/badge.svg) ![Build with Code Coverage](https://github.com/NethermindEth/nethermind/workflows/Build%20with%20Code%20Coverage/badge.svg) ![Update Documentation](https://github.com/NethermindEth/nethermind/workflows/Update%20Documentation/badge.svg) ![Publish Nethermind Image to Docker Registry](https://github.com/NethermindEth/nethermind/workflows/Publish%20Nethermind%20Image%20to%20Docker%20Registry/badge.svg) ![Publish ARM64 Image to Docker Registry](https://github.com/NethermindEth/nethermind/workflows/Publish%20ARM64%20Image%20to%20Docker%20Registry/badge.svg) | https://github.com/NethermindEth/nethermind/actions |
<!--| Travis CI | [![Build Status](https://travis-ci.org/NethermindEth/nethermind.svg?branch=master)](https://travis-ci.org/NethermindEth/nethermind) | https://travis-ci.org/NethermindEth/nethermind |-->

## Download and run:

[Windows](http://downloads.nethermind.io)<br/>
[Linux](http://downloads.nethermind.io)<br/>
[MacOS](http://downloads.nethermind.io)<br/>

It syncs fully on Mainnet, Ropsten, Rinkeby, Goerli

## Build from Source

### Prerequisites

.NET 3.0 SDK

#### Windows

* Install .NET https://www.microsoft.com/net/download
* You may need to install https://support.microsoft.com/en-us/help/2977003/the-latest-supported-visual-c-downloads

#### Linux (Ubuntu)

* Install .NET
```sh
wget https://packages.microsoft.com/config/ubuntu/$(lsb_release -rs)/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
sudo apt update && apt install dotnet-sdk-3.1
```
* `sudo apt install libsnappy-dev libc6-dev libc6`

Instructions for other distributions such as CentOS, Fedora or Debian are available here https://nethermind.readthedocs.io/en/latest/build.html

#### Mac

* Install .NET https://www.microsoft.com/net/download
* `brew install gmp snappy lz4 zstd`
* Additionally, if you have problems with start `brew install rocksdb`

### Build

```
git clone https://github.com/NethermindEth/nethermind --recursive
cd nethermind/src/Nethermind
dotnet build Nethermind.sln -c Release
cd Nethermind.Runner
dotnet run -c Release --no-build -- --config mainnet
```

### Test

if you want to run the Nethermind or Ethereum Foundation tests, then:
```
dotnet build Nethermind.sln -c Debug
dotnet test Nethermind.sln

dotnet build EthereumTests.sln -c Debug
dotnet test EthereumTests.sln
```

### IDE

* JetBrains Rider ([Link](https://www.jetbrains.com/rider))
* Visual Studio Code ([Link](https://code.visualstudio.com/docs/other/dotnet))


## Contributors welcome
[![GitHub Issues](https://img.shields.io/github/issues/nethermindeth/nethermind.svg)](https://github.com/NethermindEth/nethermind/issues)
[![Gitter](https://img.shields.io/gitter/room/nethermindeth/nethermind.svg)](https://gitter.im/nethermindeth/nethermind)
[![GitHub Contributors](https://img.shields.io/github/contributors/nethermindeth/nethermind.svg)](https://github.com/NethermindEth/nethermind/graphs/contributors)

At Nethermind we are building an open source multiplatform Ethereum client implementation in .NET Core (running seamlessly on Linux, Windows and MacOS). Simultaneously our team works on Nethermind Data Marketplace and on-chain data extraction tools and client customizations.

Nethermind client can be used in your projects, when setting up private Ethereum networks or dApps. The latest prod version of Nethermind can be found at downloads.nethermind.io.
# Links
https://nethermind.io/

# License
[![GitHub](https://img.shields.io/github/license/nethermindeth/nethermind.svg)](https://github.com/NethermindEth/nethermind/blob/master/LICENSE)

