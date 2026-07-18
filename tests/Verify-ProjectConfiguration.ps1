Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

$repoRoot = Split-Path -Parent $PSScriptRoot

function Read-RepoFile([string]$relativePath) {
    return Get-Content -Raw -LiteralPath (Join-Path $repoRoot $relativePath)
}

function Assert-Matches([string]$text, [string]$pattern, [string]$message) {
    if ($text -notmatch $pattern) {
        throw $message
    }
}

function Assert-NotMatches([string]$text, [string]$pattern, [string]$message) {
    if ($text -match $pattern) {
        throw $message
    }
}

function Assert-XmlFile([string]$relativePath) {
    $xml = New-Object System.Xml.XmlDocument
    $xml.PreserveWhitespace = $true
    $xml.Load((Join-Path $repoRoot $relativePath))
    return $xml
}

function Assert-ProjectProperty([string]$relativePath, [string]$condition, [string]$propertyName, [string]$expectedValue, [string]$message) {
    $xml = Assert-XmlFile $relativePath
    $namespaceManager = New-Object System.Xml.XmlNamespaceManager($xml.NameTable)
    $namespaceManager.AddNamespace('msb', 'http://schemas.microsoft.com/developer/msbuild/2003')

    $propertyGroup = $xml.SelectSingleNode("//msb:PropertyGroup[@Condition=`"$condition`"]", $namespaceManager)
    if ($null -eq $propertyGroup) {
        throw "Missing project property group: $condition"
    }

    $property = $propertyGroup.SelectSingleNode("msb:$propertyName", $namespaceManager)
    if ($null -eq $property -or $property.InnerText -ne $expectedValue) {
        throw $message
    }
}

$solution = Read-RepoFile 'SldWorksLookup.sln'
$project = Read-RepoFile 'SldWorksLookup\SldWorksLookup.csproj'
$testsProject = Read-RepoFile 'tests\SldWorksLookup.RegressionTests\SldWorksLookup.RegressionTests.csproj'
$install = Read-RepoFile 'SldWorksLookup\Install.bat'
$uninstall = Read-RepoFile 'SldWorksLookup\UnInstall.bat'
$installer = Read-RepoFile 'Installer\SolidWorksLookup.aip'

Assert-Matches $solution 'Release\|Any CPU\.ActiveCfg = Release\|Any CPU' 'Release|Any CPU must map to project Release|Any CPU.'
Assert-Matches $solution 'Release\|x64\.ActiveCfg = Release\|x64' 'Release|x64 must map to project Release|x64.'
Assert-NotMatches $solution 'Release\|Any CPU\.(ActiveCfg|Build\.0) = Debug\|' 'Release|Any CPU must not build a Debug project configuration.'
Assert-NotMatches $solution 'Release\|x64\.(ActiveCfg|Build\.0) = Debug\|' 'Release|x64 must not build a Debug project configuration.'

Assert-Matches $project '<XCadRegDll\s+Condition="''\$\(XCadRegDll\)'' == ''''">false</XCadRegDll>' 'XCadRegDll must default to false while allowing command-line overrides.'
Assert-ProjectProperty 'SldWorksLookup\SldWorksLookup.csproj' ' ''$(Configuration)|$(Platform)'' == ''Release|AnyCPU'' ' 'OutputPath' '..\bin\' 'Release|AnyCPU output must refresh the root bin directory used by the installer.'

Assert-Matches $install 'cd /d "%~dp0" \|\| exit /b 1' 'Install.bat must run from its own directory.'
Assert-Matches $install 'if not exist "%~dp0RegAsm\.exe" exit /b 2' 'Install.bat must verify RegAsm.exe exists.'
Assert-Matches $install 'if not exist "%~dp0SldWorksLookup\.dll" exit /b 3' 'Install.bat must verify SldWorksLookup.dll exists.'
Assert-Matches $install '"%~dp0RegAsm\.exe" "%~dp0SldWorksLookup\.dll" /codebase' 'Install.bat must quote RegAsm and target paths.'
Assert-Matches $install 'if errorlevel 1 exit /b %errorlevel%' 'Install.bat must propagate RegAsm failures.'
Assert-Matches $install 'exit /b 0' 'Install.bat must return 0 only after success.'

Assert-Matches $uninstall 'cd /d "%~dp0" \|\| exit /b 1' 'UnInstall.bat must run from its own directory.'
Assert-Matches $uninstall 'if not exist "%~dp0RegAsm\.exe" exit /b 2' 'UnInstall.bat must verify RegAsm.exe exists.'
Assert-Matches $uninstall 'if not exist "%~dp0SldWorksLookup\.dll" exit /b 3' 'UnInstall.bat must verify SldWorksLookup.dll exists.'
Assert-Matches $uninstall '"%~dp0RegAsm\.exe" "%~dp0SldWorksLookup\.dll" /u' 'UnInstall.bat must quote RegAsm and target paths.'
Assert-Matches $uninstall 'if errorlevel 1 exit /b %errorlevel%' 'UnInstall.bat must propagate RegAsm failures.'
Assert-Matches $uninstall 'exit /b 0' 'UnInstall.bat must return 0 only after success.'

Assert-Matches $installer 'AI_REQUIRED_DOTNET_DISPLAY".*4\.7\.2' 'Installer display minimum .NET version must be 4.7.2.'
Assert-Matches $installer 'AI_REQUIRED_DOTNET_VERSION".*4\.7\.2' 'Installer launch condition minimum .NET version must be 4.7.2.'
Assert-NotMatches $installer 'File="exceptionless\.txt"' 'Installer must not include an explicit exceptionless.txt file row.'
Assert-NotMatches $installer 'File="[^"]+\.(pdb|xml)"' 'Installer must not include explicit PDB or XML file rows.'
Assert-Matches $installer 'ExcludePattern="[^"]*(^|[|])exceptionless\.txt([|]|")' 'Synchronized bin content must exclude exceptionless.txt.'
Assert-Matches $installer 'ExcludePattern="[^"]*(^|[|])\*\.pdb([|]|")' 'Synchronized bin content must exclude PDB files.'
Assert-Matches $installer 'ExcludePattern="[^"]*(^|[|])\*\.xml([|]|")' 'Synchronized bin content must exclude XML documentation files.'

Assert-Matches $testsProject '<PlatformTarget>x64</PlatformTarget>' 'Regression test runner must target x64 to avoid MSB3270.'

[void](Assert-XmlFile 'SldWorksLookup\SldWorksLookup.csproj')
[void](Assert-XmlFile 'tests\SldWorksLookup.RegressionTests\SldWorksLookup.RegressionTests.csproj')
[void](Assert-XmlFile 'Installer\SolidWorksLookup.aip')

Write-Host 'Project configuration contract verified.'
