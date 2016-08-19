$assemblyFile = "$env:APPVEYOR_BUILD_FOLDER\RocksmithToolkitLib\Properties\AssemblyInfo.cs"

$regex = new-object System.Text.RegularExpressions.Regex ('(AssemblyInformationalVersion(Attribute)?\s*\(\s*\")(.*)(\"\s*\))', 
         [System.Text.RegularExpressions.RegexOptions]::MultiLine)

$content = [IO.File]::ReadAllText($assemblyFile)

$version = $null
$match = $regex.Match($content)
if($match.Success) {
    $version = $match.groups[3].value
}

# new version
$env:GIT_HASH = $env:APPVEYOR_REPO_COMMIT.Substring(0, 8)
$version = "$version.$env:GIT_HASH"

# update assembly info
$content = $regex.Replace($content, '${1}' + $version + '${4}')
[IO.File]::WriteAllText($assemblyFile, $content)

# update AppVeyor build
Update-AppveyorBuild -Version $version
