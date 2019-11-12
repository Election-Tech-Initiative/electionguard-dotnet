#!/bin/bash

SOURCE="../../libs/ElectionGuard-SDK-C-Implementation/build" #location of starting directory
DESTINATION="../../libs/electionguard/"; #location where files will be copied to
FILES="*electionguard*"

echo "Remove outdated library files"
rm -rf -v $DESTINATION

echo "Validating library directory"
mkdir -p $DESTINATION

echo "Move build files to library"

find $SOURCE -type f -name $FILES -exec cp '{}' $DESTINATION ';'