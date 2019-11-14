# Download latest release from github

$repo = "microsoft/ElectionGuard-SDK-C-Implementation"
$file = "electionguard.zip"

Write-Host Determining latest release
$tag = git ls-remote --tags --sort="v:refname" git://github.com/$repo.git | tail -n1 | sed 's/.*\///; s/\^{}//'
Write-Host "Latest release tag found: $tag"

$download = "https://github.com/$repo/releases/download/$tag/$file"
$name = $file.Split(".")[0]
$zip = "$name-$tag.zip"
$dir = "$name-$tag"
$dll = "$name.dll"
$so = "lib$name.so"
$dylib = "lib$name.dylib"
$lib_path = "..\..\libs\electionguard"

Write-Host Downloading latest release
Write-Host $download
Invoke-WebRequest $download -Out $zip

Write-Host Extracting release files
Expand-Archive $zip -Force

Write-Host Remove outdated library files
Remove-Item $lib_path -Recurse -Force -ErrorAction SilentlyContinue 

Write-Host Validating library directory
New-Item -ItemType Directory -Force -Path $lib_path

Write-Host Move release files to library
Move-Item $dir\$dll\$dll -Destination $lib_path\$dll -Force
Move-Item $dir\$so\$so -Destination $lib_path\$so -Force
Move-Item $dir\$dylib\$dylib -Destination $lib_path\$dylib -Force

Write-Host Deleting temp files
Remove-Item $zip -Force
Remove-Item $dir -Recurse -Force