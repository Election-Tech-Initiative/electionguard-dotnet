$source="../../libs/ElectionGuard-SDK-C-Implementation/build" #location of starting directory
$destination="../../libs/electionguard"; #location where files will be copied to
$files=@("*electionguard*")

Write-Host Remove outdated library files
Remove-Item $destination -Recurse -Force -ErrorAction SilentlyContinue 

Write-Host Validating library source and destination
New-Item -ItemType Directory -Force -Path $source
New-Item -ItemType Directory -Force -Path $destination

Write-Host Move build files to library
New-Item -ItemType Directory -Force -Path ($destination); Get-ChildItem -recurse ($source) -include ($files) | Copy-Item -Destination ($destination)