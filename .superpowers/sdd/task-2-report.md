# Task 2 Report: COM lifetime and actionable exception reporting

## RED

- Command:
  `msbuild .\tests\SldWorksLookup.RegressionTests\SldWorksLookup.RegressionTests.csproj /p:Configuration=Release /p:XCadRegDll=false /v:minimal /nologo`
- Result: failed as expected after adding Task 2 tests.
- Evidence:
  - `CS0103`: `SelectionAccessScope` did not exist.
  - `CS0103`: `ExceptionUtil` did not exist.
  - `CS0117`: `LogExtension.TryReadConfiguration` did not exist.

- Review-fix command:
  `msbuild .\tests\SldWorksLookup.RegressionTests\SldWorksLookup.RegressionTests.csproj /p:Configuration=Release /p:XCadRegDll=false /v:minimal /nologo`
- Result: failed as expected after adding construction-cleanup tests.
- Evidence:
  - `CS0103`: `ConstructionCleanup` did not exist.

## GREEN

- Command:
  `msbuild .\tests\SldWorksLookup.RegressionTests\SldWorksLookup.RegressionTests.csproj /p:Configuration=Release /p:XCadRegDll=false /v:minimal /nologo`
- Result: passed.

- Command:
  `.\tests\SldWorksLookup.RegressionTests\bin\Release\net472\SldWorksLookup.RegressionTests.exe`
- Result: passed 18 regression tests, including all Task 1 and Task 2 tests.

- Command:
  `msbuild .\SldWorksLookup.sln /p:Configuration=Release '/p:Platform=Any CPU' /p:XCadRegDll=false /v:minimal /nologo`
- Result: passed. Remaining warning is the existing test-project x86/AMD64 processor architecture mismatch, not CS0168.

- Command:
  `msbuild .\SldWorksLookup.sln /p:Configuration=Release /p:Platform=x64 /p:XCadRegDll=false /v:minimal /nologo`
- Result: passed. Remaining warning is the existing test-project x86/AMD64 processor architecture mismatch, not CS0168.

- Command:
  `git diff --check`
- Result: passed. Git reported CRLF normalization warnings only.

## Files

- Created `SldWorksLookup/Helper/SelectionAccessScope.cs`.
- Created `SldWorksLookup/Helper/ExceptionUtil.cs`.
- Created `SldWorksLookup/Helper/ConstructionCleanup.cs`.
- Modified `tests/SldWorksLookup.RegressionTests/Program.cs`.
- Modified `SldWorksLookup/AddIn.cs`.
- Modified `SldWorksLookup/LogExtension.cs`.
- Modified `SldWorksLookup/Model/Value/LookupValue.cs`.
- Modified `SldWorksLookup/ViewModel/CaptureCmdViewModel.cs`.
- Modified `SldWorksLookup/View/CaptureCmd.xaml.cs`.
- Modified `SldWorksLookup/ViewModel/GetObjectByPIDWindowViewModel.cs`.
- Modified `SldWorksLookup/SldWorksLookup.csproj`.

## Risks

- SolidWorks COM UI paths were build-verified and covered where dependency-free tests can reach them, but not manually exercised in a live SolidWorks session.
- Solution builds still report an existing architecture mismatch warning for the regression test executable referencing the x64 add-in assembly.
- `AGENTS.md` was not present in this worktree root; the task-provided AGENTS instructions were followed.
