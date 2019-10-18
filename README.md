
# ElectionGuard CSharp

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

This project welcomes contributions and suggestions.  Most contributions require you to agree to a
Contributor License Agreement (CLA) declaring that you have the right to, and actually do, grant us
the rights to use your contribution. For details, visit https://cla.opensource.microsoft.com.

When you submit a pull request, a CLA bot will automatically determine whether you need to provide
a CLA and decorate the PR appropriately (e.g., status check, comment). Simply follow the instructions
provided by the bot. You will only need to do this once across all repos using our CLA.

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/).
For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or
contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.
