# Task 4 Report: Reproducible and safer build/installer configuration

## RED

Created `tests/Verify-ProjectConfiguration.ps1` as a configuration contract test covering:

- Release solution mappings must map to Release project configurations and must not map to Debug.
- `SldWorksLookup.csproj` must default `XCadRegDll` to `false` while allowing command-line override.
- install/uninstall batch files must run from the script directory, quote paths, check required files, propagate `RegAsm` errorlevel, and return `0` only after success.
- installer .NET minimum must be 4.7.2.
- installer must not include explicit `exceptionless.txt`, `*.pdb`, or `*.xml` file rows.
- synchronized `..\bin` content must exclude `exceptionless.txt`, `*.pdb`, and `*.xml`.
- regression test runner must target x64.
- project and AIP XML must parse.

Command:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .\tests\Verify-ProjectConfiguration.ps1
```

Observed RED:

```text
Release|x64 must map to project Release|x64.
```

The same pre-fix state also lacked the `XCadRegDll=false` default and safe batch/installer contract.

## GREEN

Implemented:

- Mapped add-in solution `Release|Any CPU` and `Release|x64` to project Release configurations instead of Debug.
- Mapped regression test solution x64 configurations to project x64 and set the SDK test runner `PlatformTarget` to x64 to remove the known `MSB3270` mismatch.
- Added `<XCadRegDll Condition="'$(XCadRegDll)' == ''">false</XCadRegDll>` so normal builds are side-effect free while `/p:XCadRegDll=true` still wins.
- Hardened `Install.bat` and `UnInstall.bat` with script-directory execution, quoted paths, file existence checks, raw `RegAsm` errorlevel propagation, and explicit success exit.
- Raised Advanced Installer launch-condition/display minimum .NET to 4.7.2.
- Removed explicit installer rows for `exceptionless.txt`, `.pdb`, and `.xml` files.
- Updated the existing Advanced Installer synchronized folder `ExcludePattern` with `exceptionless.txt|*.pdb|*.xml`.

## Verification

Commands and results:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .\tests\Verify-ProjectConfiguration.ps1
```

Result: exited `0`, `Project configuration contract verified.`

```powershell
& $msbuild .\SldWorksLookup.sln /t:Restore /p:RestorePackagesConfig=true /v:minimal /nologo
```

Result: exited `0`.

```powershell
& $msbuild .\SldWorksLookup.sln /p:Configuration=Release '/p:Platform=Any CPU' /v:normal /nologo
```

Result: exited `0`. Captured output contained no `RegAsm`, `regsvr32`, `MSB3270`, or `warning` matches, proving the default `XCadRegDll=false` build did not run registration.

```powershell
& $msbuild .\SldWorksLookup.sln /p:Configuration=Release /p:Platform=x64 /p:XCadRegDll=false /v:minimal /nologo
```

Result: exited `0`.

```powershell
& $msbuild .\SldWorksLookup\SldWorksLookup.csproj /getProperty:XCadRegDll /nologo
& $msbuild .\SldWorksLookup\SldWorksLookup.csproj /getProperty:XCadRegDll /p:XCadRegDll=true /nologo
```

Results: default printed `false`; command-line override printed `true`.

```powershell
.\tests\SldWorksLookup.RegressionTests\bin\Release\net472\SldWorksLookup.RegressionTests.exe
.\tests\SldWorksLookup.RegressionTests\bin\x64\Release\net472\SldWorksLookup.RegressionTests.exe
```

Results: both executables exited `0`; all 26 regression tests passed in each location.

```powershell
git diff --check
```

Result: exited `0`.

## Notes

- `AdvancedInstaller.com` is not installed on this machine, so no `.aip` build was attempted.
- Installer validation was limited to XML parsing plus the configuration contract test against the actual AIP XML structure.
