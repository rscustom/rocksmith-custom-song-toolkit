Param(
    [Parameter(Mandatory=$true)][string] $version,
)

Write-Host "Applying version ${version} to AssemblyInfo.cs"

foreach ($file in "AssemblyInfo.cs" )
{
	$env:APPVEYOR_BUILD_FOLDER | get-childitem -recurse |? {$_.Name -eq $file} | Update-SourceVersion $version;
}