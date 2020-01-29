.PHONY: build run test

# a phony dependency that can be used as a dependency to force builds
FORCE:

build: FORCE
	cd libs/ElectionGuard-SDK-C-Implementation && make -f Makefile.mk build
	dotnet build

run: build
	cd tests/TestApp && ./bin/Debug/netcoreapp3.0/TestApp

test: build
	dotnet test
