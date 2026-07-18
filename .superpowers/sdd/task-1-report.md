# Task 1 Report: Dependency-free test runner and path reliability

## Status

Complete.

## Files

- Created `tests/SldWorksLookup.RegressionTests/SldWorksLookup.RegressionTests.csproj`
- Created `tests/SldWorksLookup.RegressionTests/Program.cs`
- Created `SldWorksLookup/PathSplit/SketchChainTopology.cs`
- Created `SldWorksLookup/PathSplit/SegmentSamplingPlan.cs`
- Modified `SldWorksLookup/PathSplit/SketchWrapper.cs`
- Modified `SldWorksLookup/PathSplit/SketchSegmentWrapper.cs`
- Modified `SldWorksLookup/PathSplit/SketchChain.cs`
- Modified `SldWorksLookup/PathSplit/ExtensionMethods.cs`
- Modified `SldWorksLookup/Helper/PathExportUtil.cs`
- Modified `SldWorksLookup/Properties/AssemblyInfo.cs`
- Modified `SldWorksLookup/SldWorksLookup.csproj`
- Modified `SldWorksLookup.sln`

## RED

Initial command:

```powershell
& 'C:\Program Files\Microsoft Visual Studio\18\Community\MSBuild\Current\Bin\MSBuild.exe' .\tests\SldWorksLookup.RegressionTests\SldWorksLookup.RegressionTests.csproj /p:Configuration=Release /p:XCadRegDll=false /v:minimal /nologo
```

Initial result: failed before compilation because the SDK-style project needed a restore-generated `project.assets.json`.

Restore prerequisite:

```powershell
& 'C:\Program Files\Microsoft Visual Studio\18\Community\MSBuild\Current\Bin\MSBuild.exe' .\tests\SldWorksLookup.RegressionTests\SldWorksLookup.RegressionTests.csproj /t:Restore /p:Configuration=Release /p:XCadRegDll=false /v:minimal /nologo
```

Restore output:

```text
正在确定要还原的项目…
已还原 ...\tests\SldWorksLookup.RegressionTests\SldWorksLookup.RegressionTests.csproj
```

Correct RED command:

```powershell
& 'C:\Program Files\Microsoft Visual Studio\18\Community\MSBuild\Current\Bin\MSBuild.exe' .\tests\SldWorksLookup.RegressionTests\SldWorksLookup.RegressionTests.csproj /p:Configuration=Release /p:XCadRegDll=false /v:minimal /nologo
```

Correct RED output excerpt:

```text
SldWorksLookup -> ...\SldWorksLookup\bin\Release\SldWorksLookup.dll
Program.cs(75,24): error CS0103: 当前上下文中不存在名称“SegmentSamplingPlan”
Program.cs(84,61): error CS0103: 当前上下文中不存在名称“SegmentSamplingPlan”
Program.cs(85,61): error CS0103: 当前上下文中不存在名称“SegmentSamplingPlan”
Program.cs(86,61): error CS0103: 当前上下文中不存在名称“SegmentSamplingPlan”
Program.cs(87,61): error CS0103: 当前上下文中不存在名称“SegmentSamplingPlan”
Program.cs(92,20): error CS0103: 当前上下文中不存在名称“SketchChainTopology”
```

The RED failure was expected because the tests referenced the missing topology and sampling helpers.

## GREEN

Build command:

```powershell
& 'C:\Program Files\Microsoft Visual Studio\18\Community\MSBuild\Current\Bin\MSBuild.exe' .\tests\SldWorksLookup.RegressionTests\SldWorksLookup.RegressionTests.csproj /p:Configuration=Release /p:XCadRegDll=false /v:minimal /nologo
```

Build output:

```text
SldWorksLookup -> ...\SldWorksLookup\bin\Release\SldWorksLookup.dll
SldWorksLookup.RegressionTests -> ...\tests\SldWorksLookup.RegressionTests\bin\Release\net472\SldWorksLookup.RegressionTests.exe
```

Runner command:

```powershell
& .\tests\SldWorksLookup.RegressionTests\bin\Release\net472\SldWorksLookup.RegressionTests.exe
```

Runner output:

```text
PASS PathTopologyReturnsEveryDisconnectedSegment
PASS PathTopologyReturnsClosedLoop
PASS SamplingPlanCarriesSpacingAcrossShortSegment
PASS SamplingPlanRejectsNonPositiveOrNonFiniteStep
```

## Self-review

- `SketchChainTopology.Build` consumes all disconnected chains and closed-loop chains without mutating the source list.
- `SegmentSamplingPlan.Create` follows the requested spacing carry formula and rejects non-positive or non-finite step lengths.
- `SketchWrapper.GetChains` now delegates ordering/reversal to the topology helper.
- `SketchSegmentWrapper.SplitCurve` uses distance-based sampling and normalized curve parameter interpolation, avoiding divide-by-zero behavior from the old spare-length logic.
- `SketchChain.Split` validates step length once and no longer rejects short segments.
- `PathExportUtil` reuses `SketchSegmentWrapper.SourceStartPoint` and `SourceEndPoint`, removing duplicated endpoint switching logic.
- COM dereferences in `PathExportUtil` are guarded for active document, selection, sketch, sketch segments, generated paths, path segments, and segment curves.
- `ExtensionMethods.GetSkeFeat` now inspects and yields `subfeat` inside the subfeature loop.
- `git diff --check` exits 0; warnings only report LF-to-CRLF normalization.
- No temporary debug code was added. The only `Console.WriteLine` calls are the dependency-free test runner output.

## Concerns

- Build still emits pre-existing CS0168 warnings in `SldWorksLookup/AddIn.cs` and `SldWorksLookup/LogExtension.cs`; those files are outside Task 1 ownership and were not changed.
- The SolidWorks COM export path was build-verified but not manually exercised in SolidWorks.

## CHANGES_REQUESTED Follow-up

### Follow-up RED

Added tests for:

- first segment reversal
- connected successor reversal
- per-segment continuity
- preserving original input list membership/order
- normal sampling
- `distanceToNextPoint == segmentLength`
- immediate contextual export merge failure

Command:

```powershell
& 'C:\Program Files\Microsoft Visual Studio\18\Community\MSBuild\Current\Bin\MSBuild.exe' .\tests\SldWorksLookup.RegressionTests\SldWorksLookup.RegressionTests.csproj /p:Configuration=Release /p:XCadRegDll=false /v:minimal /nologo
```

RED output excerpt:

```text
SldWorksLookup -> ...\SldWorksLookup\bin\Release\SldWorksLookup.dll
Program.cs(185,40): error CS0117: “PathExportUtil”未包含“MergeCurveOrThrow”的定义
Program.cs(189,38): error CS0117: “PathExportUtil”未包含“MergeCurveOrThrow”的定义
```

The RED failure was expected because the new dependency-free export state helper did not exist.

### Follow-up GREEN

Build command:

```powershell
& 'C:\Program Files\Microsoft Visual Studio\18\Community\MSBuild\Current\Bin\MSBuild.exe' .\tests\SldWorksLookup.RegressionTests\SldWorksLookup.RegressionTests.csproj /p:Configuration=Release /p:XCadRegDll=false /v:minimal /nologo
```

Build output excerpt:

```text
SldWorksLookup -> ...\SldWorksLookup\bin\Release\SldWorksLookup.dll
SldWorksLookup.RegressionTests -> ...\tests\SldWorksLookup.RegressionTests\bin\Release\net472\SldWorksLookup.RegressionTests.exe
```

Runner command:

```powershell
& .\tests\SldWorksLookup.RegressionTests\bin\Release\net472\SldWorksLookup.RegressionTests.exe
```

Runner output:

```text
PASS PathTopologyReturnsEveryDisconnectedSegment
PASS PathTopologyReturnsClosedLoop
PASS PathTopologyReversesOpenFirstSegment
PASS PathTopologyReversesConnectedSuccessor
PASS PathTopologyKeepsEveryStepContinuous
PASS PathTopologyDoesNotReorderOrRemoveInputSegments
PASS SamplingPlanCarriesSpacingAcrossShortSegment
PASS SamplingPlanRejectsNonPositiveOrNonFiniteStep
PASS SamplingPlanReturnsNormalSpacing
PASS SamplingPlanHandlesExactCarryBoundary
PASS ExportMergeFailureThrowsContext
```

Additional verification:

```text
git diff --check
```

Result: exit 0; warnings only report LF-to-CRLF normalization.

### Follow-up Self-review

- `PathExportUtil` now guards the result of `CreateTrimmedCurve2`, `CreateWireBody`, `doc as PartDoc`, and `MergeCurves`.
- Merge failure is handled immediately by `MergeCurveOrThrow` with contextual `InvalidOperationException`; a null merge cannot silently become the next segment on a later loop.
- `SketchChainTopology.Build<T>` and its private helpers now use `where T : class`, matching the `ReferenceEquals` identity semantics.
- No AssemblyInfo version values changed in either commit; the only AssemblyInfo change remains `InternalsVisibleTo("SldWorksLookup.RegressionTests")`.
