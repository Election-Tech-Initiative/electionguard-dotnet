#!/bin/bash

REPO="microsoft/ElectionGuard-SDK-C-Implementation"
FILE="electionguard.zip"

RELEASES="https://api.github.com/repos/$REPO/releases"

echo "Determining latest release"
RELEASES_REQUEST="curl --silent --url ""$RELEASES"" --header 'Authorization: token $GITHUB_TOKEN'"
# Uncomment lines below if we need to debug the response
# RELEASES_RESPONSE=$($RELEASES_REQUEST)
# echo "Latest release response JSON: $RELEASES_RESPONSE"
TAG=$($RELEASES_REQUEST | grep -m1 '"tag_name":' | sed -E 's/.*"([^"]+)".*/\1/')
echo "Latest release tag found: $TAG"

DOWNLOAD="https://github.com/$REPO/releases/download/$TAG/$FILE"
NAME="electionguard"
ZIP="$NAME-$TAG.zip"
DIR="$NAME-$TAG"
DLL="$NAME.dll"
SO="lib$NAME.so"
DYLIB="lib$NAME.dylib"
LIB_PATH="../../libs/electionguard"

echo "Downloading latest release"
curl -L $DOWNLOAD -o $ZIP

echo "Extracting release files"
sudo apt-get install unzip
unzip -o $ZIP -d $DIR

echo "Remove outdated library files"
rm -rf -v $LIB_PATH

echo "Validating library directory"
mkdir -p $LIB_PATH

echo "Move release files to library"
mv -f -v $DIR/$DLL/$DLL "$LIB_PATH/$DLL"
mv -f -v $DIR/$SO/$SO $LIB_PATH/$SO
mv -f -v $DIR/$DYLIB/$DYLIB $LIB_PATH/$DYLIB

echo "Deleting temp files"
rm -rf -v $ZIP
rm -rf -v $DIR
