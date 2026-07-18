# SolidWorksLookup Reliability Fixes Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Fix the confirmed path-processing, COM lifecycle, exception-reporting, reflection, and packaging defects and publish the changes as a draft pull request against `develop`.

**Architecture:** Add a dependency-free .NET Framework regression-test executable that exercises pure path topology, resource lifetime, configuration parsing, reflection, and object-array behavior without starting SolidWorks. Keep COM-facing changes local to existing UI/add-in classes, and extract helpers only for pure algorithms or `try/finally` resource boundaries.

**Tech Stack:** C# 7.3, .NET Framework 4.7.2, WPF, SolidWorks interop, Xarial.XCad, MSBuild, PowerShell.

## Global Constraints

- Do not add NuGet or other project dependencies.
- Keep the production target at `.NET Framework 4.7.2` and `x64`.
- Tests must run without starting or connecting to SolidWorks.
- All build and test commands must set `XCadRegDll=false`; validation must not register the add-in.
- Preserve current public behavior except where this plan explicitly fixes a confirmed defect.
- Use minimal, locally readable C# changes; extract only pure algorithms and resource-lifetime boundaries.
- Every behavior change follows RED → GREEN and records the observed failing and passing command.
- Every commit follows the workspace Lore Commit Protocol.

---

### Task 1: Dependency-free test runner and path reliability

**Files:**
- Create: `tests/SldWorksLookup.RegressionTests/SldWorksLookup.RegressionTests.csproj`
- Create: `tests/SldWorksLookup.RegressionTests/Program.cs`
- Create: `SldWorksLookup/PathSplit/SketchChainTopology.cs`
- Create: `SldWorksLookup/PathSplit/SegmentSamplingPlan.cs`
- Modify: `SldWorksLookup/PathSplit/SketchWrapper.cs`
- Modify: `SldWorksLookup/PathSplit/SketchSegmentWrapper.cs`
- Modify: `SldWorksLookup/PathSplit/SketchChain.cs`
- Modify: `SldWorksLookup/PathSplit/ExtensionMethods.cs`
- Modify: `SldWorksLookup/Helper/PathExportUtil.cs`
- Modify: `SldWorksLookup/Properties/AssemblyInfo.cs`
- Modify: `SldWorksLookup/SldWorksLookup.csproj`
- Modify: `SldWorksLookup.sln`

**Interfaces:**
- Produces: `SketchChainTopology.Build<T>(IList<T>, Func<T, Point3D>, Func<T, Point3D>, Action<T>, Func<Point3D, Point3D, bool>)`
- Produces: `SegmentSamplingPlan.Create(double segmentLength, double stepLength, double distanceToNextPoint)`
- Produces: executable `tests/SldWorksLookup.RegressionTests/bin/Release/net472/SldWorksLookup.RegressionTests.exe`

- [ ] **Step 1: Create the regression runner and failing path tests**

Create an SDK-style `net472` console project with only a `ProjectReference` to the add-in project. Add a tiny runner that executes named `Action` tests and returns `1` on any failure.

The initial tests must include:

```csharp
PathTopologyReturnsEveryDisconnectedSegment();
PathTopologyReturnsClosedLoop();
SamplingPlanCarriesSpacingAcrossShortSegment();
SamplingPlanRejectsNonPositiveOrNonFiniteStep();
```

Use a local test-only segment class:

```csharp
private sealed class Segment
{
    public Segment(Point3D start, Point3D end)
    {
        Start = start;
        End = end;
    }

    public Point3D Start { get; private set; }
    public Point3D End { get; private set; }

    public void Reverse()
    {
        var start = Start;
        Start = End;
        End = start;
    }
}
```

- [ ] **Step 2: Run the tests and verify RED**

Run:

```powershell
& $msbuild .\tests\SldWorksLookup.RegressionTests\SldWorksLookup.RegressionTests.csproj `
  /p:Configuration=Release /p:XCadRegDll=false /v:minimal /nologo
```

Expected: compilation fails because `SketchChainTopology` and `SegmentSamplingPlan` do not exist.

- [ ] **Step 3: Implement topology and sampling**

`SketchChainTopology.Build` must:

```csharp
var remaining = new List<T>(segments);
while (remaining.Count > 0)
{
    // Prefer a segment with an open endpoint; if none exists, start a closed loop.
    // Reverse the first segment only when its start is connected and its end is open.
    // Repeatedly consume a segment connected to the current end, reversing it when needed.
    // Add the completed chain, then continue until remaining is empty.
}
```

`SegmentSamplingPlan.Create` must:

```csharp
if (stepLength <= 0 || double.IsNaN(stepLength) || double.IsInfinity(stepLength))
    throw new ArgumentOutOfRangeException(nameof(stepLength));
if (segmentLength < 0 || double.IsNaN(segmentLength) || double.IsInfinity(segmentLength))
    throw new ArgumentOutOfRangeException(nameof(segmentLength));

if (distanceToNextPoint > segmentLength)
    return new SegmentSamplingPlan(0, 0, distanceToNextPoint - segmentLength);

var count = (int)Math.Floor((segmentLength - distanceToNextPoint) / stepLength) + 1;
var lastDistance = distanceToNextPoint + (count - 1) * stepLength;
var nextDistance = stepLength - (segmentLength - lastDistance);
return new SegmentSamplingPlan(count, distanceToNextPoint, nextDistance);
```

Replace `SketchWrapper.GetChains` index mutation with the topology helper. Replace `SketchSegmentWrapper.SplitCurve` division-based logic with the sampling plan and normalized parameter interpolation. `SketchChain.Split` must validate the step once and allow segments shorter than the step.

Delete the duplicate endpoint switch in `PathExportUtil`; construct `SketchSegmentWrapper` and reuse its `SourceStartPoint` and `SourceEndPoint`.
Validate the active document, selected feature, sketch, segment array, and generated path array before dereferencing COM results.

In `ExtensionMethods.GetSkeFeat`, inspect and yield `subfeat` inside the subfeature loop instead of repeatedly testing and yielding the parent `feat`.

- [ ] **Step 4: Run tests and verify GREEN**

Run the test project, then execute its output:

```powershell
& $msbuild .\tests\SldWorksLookup.RegressionTests\SldWorksLookup.RegressionTests.csproj `
  /p:Configuration=Release /p:XCadRegDll=false /v:minimal /nologo
& .\tests\SldWorksLookup.RegressionTests\bin\Release\net472\SldWorksLookup.RegressionTests.exe
```

Expected: build exit `0`; four path tests pass.

- [ ] **Step 5: Commit**

Commit intent: `Preserve complete sketch paths during export`.

---

### Task 2: COM lifetime and actionable exception reporting

**Files:**
- Create: `SldWorksLookup/Helper/SelectionAccessScope.cs`
- Create: `SldWorksLookup/Helper/ExceptionUtil.cs`
- Modify: `tests/SldWorksLookup.RegressionTests/Program.cs`
- Modify: `SldWorksLookup/AddIn.cs`
- Modify: `SldWorksLookup/LogExtension.cs`
- Modify: `SldWorksLookup/Model/Value/LookupValue.cs`
- Modify: `SldWorksLookup/ViewModel/CaptureCmdViewModel.cs`
- Modify: `SldWorksLookup/View/CaptureCmd.xaml.cs`
- Modify: `SldWorksLookup/ViewModel/GetObjectByPIDWindowViewModel.cs`
- Modify: `SldWorksLookup/SldWorksLookup.csproj`

**Interfaces:**
- Produces: `SelectionAccessScope.Run(Func<bool> acquire, Action release, Action body)`
- Produces: `ExceptionUtil.GetUserMessage(Exception exception)`
- Produces: `LogExtension.TryReadConfiguration(string path, out string serverUrl, out string apiKey)`

- [ ] **Step 1: Write failing lifetime, logging, and exception tests**

Add tests that prove:

```csharp
SelectionAccessScopeReleasesWhenBodyThrows();
SelectionAccessScopeDoesNotRunBodyWhenAcquireFails();
ExceptionUtilUnwrapsTargetInvocationException();
LogConfigurationReadsTwoTrimmedValues();
LogConfigurationRejectsMissingOrIncompleteFile();
```

The release-on-throw test must catch the body exception and assert `releaseCount == 1`. Configuration tests must use a temporary file and delete it in `finally`.

- [ ] **Step 2: Run tests and verify RED**

Expected: compilation fails because the three new helper APIs do not exist.

- [ ] **Step 3: Implement minimal lifetime and reporting fixes**

`SelectionAccessScope.Run` must acquire once and always release in `finally`:

```csharp
if (!acquire())
    throw new InvalidOperationException("Cannot access the feature selections.");
try
{
    body();
}
finally
{
    release();
}
```

Use it around `IAdvancedHoleFeatureData.AccessSelections` and `ReleaseSelectionAccess`. Check `GetDefinition()` and near-side element results before enumeration.

`LogExtension.TryReadConfiguration` must return `false` for a missing file, fewer than two lines, or blank values. `LogStart` must use the helper and write initialization failures to `Debug` instead of using an empty catch.

`CmdGroup_CommandClick` and `LookupValue.OpenClick` must show `ExceptionUtil.GetUserMessage(ex)` and submit telemetry only when a client exists. Add the missing `return` after “No active doc”.

Track open `CaptureCmd` windows in `AddIn`; close them during `OnDisconnect`. Make `CaptureCmdViewModel` idempotently `IDisposable`, and call `Dispose()` from the window’s `Closed` handler.

- [ ] **Step 4: Run tests and verify GREEN**

Expected: all Task 1 and Task 2 tests pass; production build has no unused-exception warnings.

- [ ] **Step 5: Commit**

Commit intent: `Keep SolidWorks state recoverable when commands fail`.

---

### Task 3: Reflection and COM-object browsing resilience

**Files:**
- Modify: `tests/SldWorksLookup.RegressionTests/Program.cs`
- Modify: `SldWorksLookup/Helper/ObjectMatcherUtil.cs`
- Modify: `SldWorksLookup/Helper/TypeMatcherUtil.cs`
- Modify: `SldWorksLookup/Helper/TypeMatcherUtil.tt`
- Modify: `SldWorksLookup/Model/Instance/InstanceProperty.cs`
- Modify: `SldWorksLookup/Model/Instance/MethodInstanceProperty.cs`
- Modify: `SldWorksLookup/Model/Property/LookupParameterProperty.cs`
- Modify: `SldWorksLookup/Model/Value/LookupValue.cs`
- Modify: `SldWorksLookup/Model/Tree/IComponent2InstanceTree.cs`
- Modify: `SldWorksLookup/Model/Tree/IFeatureInstanceTree.cs`

**Interfaces:**
- Consumes: the regression runner from Task 1
- Produces: safe array inspection, accessor enumeration, parameter defaults, and runtime COM-class-to-interface matching

- [ ] **Step 1: Write failing reflection tests**

Add tests:

```csharp
ValueArrayInspectionHandlesNullElements();
PropertyBrowsingHandlesSetterOnlyProperty();
ReferenceParametersDefaultToNull();
ComClassWithInternalIMapsToInterface();
```

Use a local class with a setter-only property for the property test. Assert:

```csharp
ObjectMatcherUtil.IsValueArray(new object[] { null, 42 }) == true;
LookupParameterProperty.CreateInstace(typeof(string)) == null;
TypeMatcherUtil.Match(typeof(ImportDxfDwgDataClass)) == typeof(IImportDxfDwgData);
```

- [ ] **Step 2: Run tests and verify RED**

Expected: at least the null-array, reference-default, setter-only, and internal-`I` mapping assertions fail.

- [ ] **Step 3: Implement minimal reflection fixes**

Use the first non-null array element for type checks and render null items as `<NULL>`. Skip null elements when opening object arrays.

In `InstanceProperty.GetProperties`, independently inspect `GetMethod` and `SetMethod`. A property without a getter must become a message-only property rather than calling `GetValue`.

Return `null` for reference parameter defaults and allow null values for reference-type method parameters. Reject null only for non-nullable value types; validate non-null values against the effective parameter type, including by-ref element types.

Before consulting the generated tuple list, `TypeMatcherUtil.Match` must resolve:

```csharp
var interfaceType = sourceType.Assembly.GetType($"{sourceType.Namespace}.I{name}");
if (interfaceType != null && interfaceType.IsInterface)
    return interfaceType;
```

Update the T4 template to include only interfaces and strip only the leading `I`.

Set `NodeStatus = NodeStatus.Ok` after feature/component lazy loading so repeated clicks do not duplicate children.

- [ ] **Step 4: Run tests and verify GREEN**

Expected: all regression tests pass.

- [ ] **Step 5: Commit**

Commit intent: `Keep reflection browsing usable across COM edge cases`.

---

### Task 4: Reproducible and safer build/installer configuration

**Files:**
- Create: `tests/Verify-ProjectConfiguration.ps1`
- Modify: `SldWorksLookup.sln`
- Modify: `SldWorksLookup/SldWorksLookup.csproj`
- Modify: `SldWorksLookup/Install.bat`
- Modify: `SldWorksLookup/UnInstall.bat`
- Modify: `Installer/SolidWorksLookup.aip`

**Interfaces:**
- Produces: PowerShell configuration contract test
- Produces: solution `Release` mappings that build project `Release`
- Produces: builds that do not auto-register unless explicitly overridden

- [ ] **Step 1: Write the failing configuration test**

The script must read repository files and throw unless:

```powershell
$solution -match 'Release\|Any CPU\.ActiveCfg = Release\|Any CPU'
$solution -match 'Release\|x64\.ActiveCfg = Release\|x64'
$project -match '<XCadRegDll>false</XCadRegDll>'
$install -match 'if errorlevel 1 exit /b'
$uninstall -match 'if errorlevel 1 exit /b'
$installer -match 'AI_REQUIRED_DOTNET_VERSION".*4\.7\.2'
$installer -notmatch 'File="exceptionless\.txt"'
```

- [ ] **Step 2: Run the script and verify RED**

Run:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .\tests\Verify-ProjectConfiguration.ps1
```

Expected: script exits non-zero on the current Release mapping and registration settings.

- [ ] **Step 3: Fix configuration**

Map both solution Release configurations to project Release. Set `<XCadRegDll>false</XCadRegDll>` in the project so compilation is side-effect free by default.

Both batch scripts must quote paths, validate files, propagate `RegAsm` failure, and return zero only after success:

```bat
@echo off
setlocal
cd /d "%~dp0" || exit /b 1
if not exist "%~dp0RegAsm.exe" exit /b 2
if not exist "%~dp0SldWorksLookup.dll" exit /b 3
"%~dp0RegAsm.exe" "%~dp0SldWorksLookup.dll" /codebase
if errorlevel 1 exit /b %errorlevel%
exit /b 0
```

Use `/u` in the uninstall variant. Change the installer minimum runtime to `4.7.2`, remove the `exceptionless.txt` file row, and exclude `exceptionless.txt`, `*.pdb`, and `*.xml` from synchronized `bin` content.

- [ ] **Step 4: Verify GREEN and full integration**

Run:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .\tests\Verify-ProjectConfiguration.ps1
& $msbuild .\SldWorksLookup.sln /t:Restore /p:RestorePackagesConfig=true /v:minimal /nologo
& $msbuild .\SldWorksLookup.sln /p:Configuration=Release '/p:Platform=Any CPU' /p:XCadRegDll=false /v:minimal /nologo
& .\tests\SldWorksLookup.RegressionTests\bin\Release\net472\SldWorksLookup.RegressionTests.exe
```

Expected: configuration test, solution build, and all regression tests exit `0`; no COM registration runs.

- [ ] **Step 5: Commit**

Commit intent: `Make release builds safe and reproducible`.

---

### Task 5: Final verification and publication

**Files:**
- Review all changed files from `origin/develop...HEAD`

**Interfaces:**
- Produces: draft pull request targeting `weianweigan/SolidWorksLookup:develop`

- [ ] **Step 1: Run fresh full verification**

Run configuration tests, regression tests, `Release|Any CPU`, and `Release|x64` builds with `XCadRegDll=false`. Confirm `git status --short` contains only intended tracked changes before the final commit.

- [ ] **Step 2: Run whole-branch code review**

Review the complete diff for correctness, scope, exception safety, installer safety, and test adequacy. Resolve every Critical or Important finding and rerun covering tests.

- [ ] **Step 3: Publish**

Push `agent/fix-solidworkslookup-reliability` to an authenticated user fork or the upstream repository when write permission exists. Open a draft PR against `weianweigan/SolidWorksLookup:develop` describing root causes, behavior changes, validation commands, and SolidWorks runtime limitations.
