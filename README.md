![Microsoft Defending Democracy Program: ElectionGuard](images/electionguard-banner.svg)

# Note: This repository has been deprecated & transitioned
As of 06/15/2020, this repository is no longer being actively maintained. ElectionGuard development has transitioned to the [ElectionGuard-Python](https://github.com/microsoft/electionguard-python) Repo.

This repository will remain open sourced and available, but will no longer be actively maintained or updated. Updates will be posted here and on our [Main repository](https://aka.ms/electionguard) Page. This URL will become archived and read-only in Summer of 2020.

## 🗳️ ElectionGuard SDK C#

![build](https://github.com/microsoft/electionguard-dotnet/workflows/Package/badge.svg)
[![nuget](https://img.shields.io/nuget/dt/ElectionGuard.SDK)](https://www.nuget.org/packages/ElectionGuard.SDK)
[![license](https://img.shields.io/github/license/microsoft/electionguard-dotnet)](.License)

This is a C# wrapper for the core SDK that performs election functions such as vote encryption, decryption, key generation, and tallying. This code is meant to be run on voting system hardware and to be integrated into existing (or new) voting system software. The ElectionGuard SDK is meant to add end-to-end verifiability and encryption into 3rd party comprehensive voting systems. 

This repository is pre-release. We look forward to engaging with the elections, security, and software engineering communities to continue to improve it as we move towards a full release.

**This project is bound by a [Code of Conduct][].**

## Building

### C-Implementation Submodule
For each platform, the C-Implementation in the submodule must be built first.
The instructions vary per platform.

1. Initialize the submodule
2. Navigate to `libs\ElectionGuard-SDK-C-Implementation`
3. Follow platform specific instructions

#### Linux

1. Install cmake and gmp (`sudo apt-get install cmake libgmp3-dev`)
2. `cmake -S . -B build -DBUILD_SHARED_LIBS=ON`
3. `cmake --build build`
4. `libelectionguard.so` should be created

#### MacOS (.dylib)

1. Install cmake and gmp (`brew install cmake gmp`)
2. `cmake -S . -B build -DBUILD_SHARED_LIBS=ON`
3. `cmake --build build`
4. `libelectionguard.dylib` is created

#### Windows (.dll)

1. Install cmake and gmp ([Use Step 1 from Windows Instructions for C-Implementation](https://github.com/microsoft/ElectionGuard-SDK-C-Implementation/blob/master/README-windows.md))
2. `cmake -S . -B build -G "MSYS Makefiles" -DBUILD_SHARED_LIBS=ON`
3. `cmake --build build`
4. `electionguard.dll` is created

### C# Library Solution
_Note: This build will copy the library created by the submodule build._

Use Visual Studio or `dotnet build` to build.


## Testing

_Warning: Prior to testing, the submodule and the solution must be built in correct order._

Use `dotnet test` to start unit tests or Visual Studio Test Explorer.


## Contributing
Help defend democracy and **[contribute to the project][]**.

[Code of Conduct]: CODE_OF_CONDUCT.md
[Contribute to the project]: CONTRIBUTING.md
