using SldWorksLookup.PathSplit;
using SldWorksLookup.Helper;
using SldWorksLookup.Model;
using SolidWorks.Interop.sldworks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Media.Media3D;

namespace SldWorksLookup.RegressionTests
{
    internal static class Program
    {
        private static int Main()
        {
            var tests = new Action[]
            {
                PathTopologyReturnsEveryDisconnectedSegment,
                PathTopologyReturnsClosedLoop,
                PathTopologyReversesOpenFirstSegment,
                PathTopologyReversesConnectedSuccessor,
                PathTopologyKeepsEveryStepContinuous,
                PathTopologyDoesNotReorderOrRemoveInputSegments,
                SamplingPlanCarriesSpacingAcrossShortSegment,
                SamplingPlanRejectsNonPositiveOrNonFiniteStep,
                SamplingPlanRejectsInvalidCarryDistance,
                SamplingPlanReturnsNormalSpacing,
                SamplingPlanHandlesExactCarryBoundary,
                ExportMergeFailureThrowsContext,
                EditScopeExitsOnceAndRethrowsBodyException,
                EditScopeExitsOnceAfterSuccess,
                SelectionAccessScopeReleasesWhenBodyThrows,
                SelectionAccessScopeDoesNotRunBodyWhenAcquireFails,
                ExceptionUtilUnwrapsTargetInvocationException,
                LogConfigurationReadsTwoTrimmedValues,
                LogConfigurationRejectsMissingOrIncompleteFile,
                ConstructionCleanupRunsCleanupOnceWhenInitializeThrows,
                ConstructionCleanupDoesNotCleanupWhenInitializeSucceeds,
                ValueArrayInspectionHandlesNullElements,
                PropertyBrowsingHandlesSetterOnlyProperty,
                ReferenceParametersDefaultToNull,
                ComClassWithInternalIMapsToInterface,
                LazyLoadCompletionMarksOkAfterSuccess,
                LazyLoadCompletionLeavesNeedRunWhenLoadThrows,
                TypeMatcherGeneratedEntriesUseInterfacesWithSingleLeadingI,
                ValueArrayInspectionDisplaysAllNullElements
            };

            var failed = 0;
            foreach (var test in tests)
            {
                try
                {
                    test();
                    Console.WriteLine("PASS " + test.Method.Name);
                }
                catch (Exception ex)
                {
                    failed++;
                    Console.WriteLine("FAIL " + test.Method.Name);
                    Console.WriteLine(ex.GetType().Name + ": " + ex.Message);
                }
            }

            return failed == 0 ? 0 : 1;
        }

        private static void PathTopologyReturnsEveryDisconnectedSegment()
        {
            var segments = new List<Segment>
            {
                new Segment(Point(0, 0), Point(1, 0)),
                new Segment(Point(2, 0), Point(3, 0))
            };

            var chains = BuildChains(segments);

            AssertEqual(2, chains.Count, "Disconnected segment count");
            AssertEqual(1, chains[0].Count, "First chain segment count");
            AssertEqual(1, chains[1].Count, "Second chain segment count");
            AssertSame(segments[0], chains[0][0], "First chain segment");
            AssertSame(segments[1], chains[1][0], "Second chain segment");
        }

        private static void PathTopologyReturnsClosedLoop()
        {
            var segments = new List<Segment>
            {
                new Segment(Point(0, 0), Point(1, 0)),
                new Segment(Point(1, 0), Point(1, 1)),
                new Segment(Point(1, 1), Point(0, 0))
            };

            var chains = BuildChains(segments);

            AssertEqual(1, chains.Count, "Closed loop chain count");
            AssertEqual(3, chains[0].Count, "Closed loop segment count");
            AssertPointEqual(chains[0][0].Start, chains[0][2].End, "Closed loop endpoints");
        }

        private static void SamplingPlanCarriesSpacingAcrossShortSegment()
        {
            var plan = SegmentSamplingPlan.Create(0.5, 1.0, 0.75);

            AssertEqual(0, plan.PointCount, "Point count");
            AssertClose(0, plan.FirstDistance, "First distance");
            AssertClose(0.25, plan.DistanceToNextPoint, "Distance to next point");
        }

        private static void PathTopologyReversesOpenFirstSegment()
        {
            var first = new Segment(Point(1, 0), Point(0, 0));
            var second = new Segment(Point(1, 0), Point(2, 0));
            var segments = new List<Segment> { first, second };

            var chains = BuildChains(segments);

            AssertEqual(1, chains.Count, "Chain count");
            AssertSame(first, chains[0][0], "First segment instance");
            AssertPointEqual(Point(0, 0), chains[0][0].Start, "Reversed first start");
            AssertPointEqual(Point(1, 0), chains[0][0].End, "Reversed first end");
            AssertPointEqual(chains[0][0].End, chains[0][1].Start, "First-to-second continuity");
        }

        private static void PathTopologyReversesConnectedSuccessor()
        {
            var first = new Segment(Point(0, 0), Point(1, 0));
            var second = new Segment(Point(2, 0), Point(1, 0));
            var segments = new List<Segment> { first, second };

            var chains = BuildChains(segments);

            AssertEqual(1, chains.Count, "Chain count");
            AssertSame(second, chains[0][1], "Second segment instance");
            AssertPointEqual(Point(1, 0), chains[0][1].Start, "Reversed successor start");
            AssertPointEqual(Point(2, 0), chains[0][1].End, "Reversed successor end");
            AssertPointEqual(chains[0][0].End, chains[0][1].Start, "Successor continuity");
        }

        private static void PathTopologyKeepsEveryStepContinuous()
        {
            var segments = new List<Segment>
            {
                new Segment(Point(3, 0), Point(2, 0)),
                new Segment(Point(0, 0), Point(1, 0)),
                new Segment(Point(2, 0), Point(1, 0)),
                new Segment(Point(3, 0), Point(4, 0))
            };

            var chains = BuildChains(segments);

            AssertEqual(1, chains.Count, "Chain count");
            for (var i = 1; i < chains[0].Count; i++)
            {
                AssertPointEqual(chains[0][i - 1].End, chains[0][i].Start, "Continuity at segment " + i);
            }
        }

        private static void PathTopologyDoesNotReorderOrRemoveInputSegments()
        {
            var first = new Segment(Point(1, 0), Point(0, 0));
            var second = new Segment(Point(1, 0), Point(2, 0));
            var third = new Segment(Point(3, 0), Point(4, 0));
            var segments = new List<Segment> { first, second, third };

            BuildChains(segments);

            AssertEqual(3, segments.Count, "Input segment count");
            AssertSame(first, segments[0], "Input first segment");
            AssertSame(second, segments[1], "Input second segment");
            AssertSame(third, segments[2], "Input third segment");
        }

        private static void SamplingPlanRejectsNonPositiveOrNonFiniteStep()
        {
            AssertThrows<ArgumentOutOfRangeException>(() => SegmentSamplingPlan.Create(1.0, 0, 0), "Zero step");
            AssertThrows<ArgumentOutOfRangeException>(() => SegmentSamplingPlan.Create(1.0, -1.0, 0), "Negative step");
            AssertThrows<ArgumentOutOfRangeException>(() => SegmentSamplingPlan.Create(1.0, double.NaN, 0), "NaN step");
            AssertThrows<ArgumentOutOfRangeException>(() => SegmentSamplingPlan.Create(1.0, double.PositiveInfinity, 0), "Infinite step");
        }

        private static void SamplingPlanRejectsInvalidCarryDistance()
        {
            AssertThrows<ArgumentOutOfRangeException>(() => SegmentSamplingPlan.Create(1.0, 1.0, -0.1), "Negative carry distance");
            AssertThrows<ArgumentOutOfRangeException>(() => SegmentSamplingPlan.Create(1.0, 1.0, double.NaN), "NaN carry distance");
            AssertThrows<ArgumentOutOfRangeException>(() => SegmentSamplingPlan.Create(1.0, 1.0, double.PositiveInfinity), "Infinite carry distance");
        }

        private static void SamplingPlanReturnsNormalSpacing()
        {
            var plan = SegmentSamplingPlan.Create(5.0, 2.0, 1.0);

            AssertEqual(3, plan.PointCount, "Point count");
            AssertClose(1.0, plan.FirstDistance, "First distance");
            AssertClose(2.0, plan.DistanceToNextPoint, "Distance to next point");
        }

        private static void SamplingPlanHandlesExactCarryBoundary()
        {
            var plan = SegmentSamplingPlan.Create(3.0, 2.0, 3.0);

            AssertEqual(1, plan.PointCount, "Point count");
            AssertClose(3.0, plan.FirstDistance, "First distance");
            AssertClose(2.0, plan.DistanceToNextPoint, "Distance to next point");
        }

        private static void ExportMergeFailureThrowsContext()
        {
            var existing = new CurveToken("existing");
            var next = new CurveToken("next");

            var first = PathExportUtil.MergeCurveOrThrow<CurveToken>(null, next, (left, right) => new CurveToken("unused"), "segment 1");
            AssertSame(next, first, "First curve should initialize export state");

            var ex = AssertThrows<InvalidOperationException>(
                () => PathExportUtil.MergeCurveOrThrow(existing, next, (left, right) => null, "segment 2"),
                "Merge failure");

            if (!ex.Message.Contains("segment 2"))
                throw new InvalidOperationException("Merge failure should include context. Message: " + ex.Message);
        }

        private static void EditScopeExitsOnceAndRethrowsBodyException()
        {
            var enterCount = 0;
            var exitCount = 0;
            var expected = new InvalidOperationException("body failed");

            var actual = AssertThrows<InvalidOperationException>(
                () => EditScope.Run(
                    () => enterCount++,
                    () => exitCount++,
                    () => { throw expected; }),
                "Body failure");

            AssertSame(expected, actual, "Original exception");
            AssertEqual(1, enterCount, "Enter count");
            AssertEqual(1, exitCount, "Exit count");
        }

        private static void EditScopeExitsOnceAfterSuccess()
        {
            var enterCount = 0;
            var bodyCount = 0;
            var exitCount = 0;

            EditScope.Run(
                () => enterCount++,
                () => exitCount++,
                () => bodyCount++);

            AssertEqual(1, enterCount, "Enter count");
            AssertEqual(1, bodyCount, "Body count");
            AssertEqual(1, exitCount, "Exit count");
        }

        private static void SelectionAccessScopeReleasesWhenBodyThrows()
        {
            var releaseCount = 0;
            var ex = AssertThrows<InvalidOperationException>(
                () => SelectionAccessScope.Run(
                    () => true,
                    () => releaseCount++,
                    () => { throw new InvalidOperationException("body failed"); }),
                "Body failure");

            AssertEqual("body failed", ex.Message, "Body exception message");
            AssertEqual(1, releaseCount, "Release count");
        }

        private static void SelectionAccessScopeDoesNotRunBodyWhenAcquireFails()
        {
            var bodyRunCount = 0;
            var releaseCount = 0;

            AssertThrows<InvalidOperationException>(
                () => SelectionAccessScope.Run(
                    () => false,
                    () => releaseCount++,
                    () => bodyRunCount++),
                "Acquire failure");

            AssertEqual(0, bodyRunCount, "Body run count");
            AssertEqual(0, releaseCount, "Release count");
        }

        private static void ExceptionUtilUnwrapsTargetInvocationException()
        {
            var inner = new InvalidOperationException("SOLIDWORKS refused the command");
            var outer = new TargetInvocationException(inner);

            AssertEqual("SOLIDWORKS refused the command", ExceptionUtil.GetUserMessage(outer), "User message");
        }

        private static void LogConfigurationReadsTwoTrimmedValues()
        {
            var path = Path.GetTempFileName();
            try
            {
                File.WriteAllLines(path, new[] { "  https://logs.example.test  ", "  api-key  " });

                string serverUrl;
                string apiKey;
                var result = LogExtension.TryReadConfiguration(path, out serverUrl, out apiKey);

                AssertEqual(true, result, "Configuration result");
                AssertEqual("https://logs.example.test", serverUrl, "Server URL");
                AssertEqual("api-key", apiKey, "API key");
            }
            finally
            {
                File.Delete(path);
            }
        }

        private static void LogConfigurationRejectsMissingOrIncompleteFile()
        {
            string serverUrl;
            string apiKey;
            var missingPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N") + ".txt");
            AssertEqual(false, LogExtension.TryReadConfiguration(missingPath, out serverUrl, out apiKey), "Missing file");

            var path = Path.GetTempFileName();
            try
            {
                File.WriteAllLines(path, new[] { "https://logs.example.test" });
                AssertEqual(false, LogExtension.TryReadConfiguration(path, out serverUrl, out apiKey), "Incomplete file");

                File.WriteAllLines(path, new[] { "https://logs.example.test", " " });
                AssertEqual(false, LogExtension.TryReadConfiguration(path, out serverUrl, out apiKey), "Blank api key");
            }
            finally
            {
                File.Delete(path);
            }
        }

        private static void ConstructionCleanupRunsCleanupOnceWhenInitializeThrows()
        {
            var cleanupCount = 0;
            var expected = new InvalidOperationException("owner failed");

            var actual = AssertThrows<InvalidOperationException>(
                () => ConstructionCleanup.Run(
                    () => { throw expected; },
                    () => cleanupCount++),
                "Initialize failure");

            AssertSame(expected, actual, "Original exception");
            AssertEqual(1, cleanupCount, "Cleanup count");
        }

        private static void ConstructionCleanupDoesNotCleanupWhenInitializeSucceeds()
        {
            var cleanupCount = 0;
            var initializeCount = 0;

            ConstructionCleanup.Run(
                () => initializeCount++,
                () => cleanupCount++);

            AssertEqual(1, initializeCount, "Initialize count");
            AssertEqual(0, cleanupCount, "Cleanup count");
        }

        private static void ValueArrayInspectionHandlesNullElements()
        {
            var values = new object[] { null, 42 };

            AssertEqual(true, ObjectMatcherUtil.IsValueArray(values), "Null-leading value array");

            var lookup = LookupValue.CreateValue(values, typeof(object[]));
            AssertEqual("<NULL>,42", lookup.ValueName, "Null-leading value array display");
        }

        private static void PropertyBrowsingHandlesSetterOnlyProperty()
        {
            var instanceProperty = InstanceProperty.Create(new SetterOnlyPropertyOwner(), typeof(SetterOnlyPropertyOwner));
            var property = instanceProperty.Properties.Properties
                .FirstOrDefault(p => p.DisplayName == nameof(SetterOnlyPropertyOwner.WriteOnly));

            if (property == null)
                throw new InvalidOperationException("Setter-only property was not surfaced.");

            AssertEqual("Write-only property", property.Value as string, "Setter-only property message");
        }

        private static void ReferenceParametersDefaultToNull()
        {
            AssertEqual(null, LookupParameterProperty.CreateInstace(typeof(string)), "String default");
        }

        private static void ComClassWithInternalIMapsToInterface()
        {
            AssertSame(typeof(IImportDxfDwgData), TypeMatcherUtil.Match(typeof(ImportDxfDwgDataClass)), "Import DXF/DWG interface");
        }

        private static void LazyLoadCompletionMarksOkAfterSuccess()
        {
            var nodeStatus = NodeStatus.NeedRun;
            var loadCount = 0;

            LazyLoadCompletion.Run(
                () => loadCount++,
                () => nodeStatus = NodeStatus.Ok);

            AssertEqual(1, loadCount, "Load count");
            AssertEqual(NodeStatus.Ok, nodeStatus, "Node status");
        }

        private static void LazyLoadCompletionLeavesNeedRunWhenLoadThrows()
        {
            var nodeStatus = NodeStatus.NeedRun;
            var markOkCount = 0;

            AssertThrows<InvalidOperationException>(
                () => LazyLoadCompletion.Run(
                    () => { throw new InvalidOperationException("load failed"); },
                    () =>
                    {
                        markOkCount++;
                        nodeStatus = NodeStatus.Ok;
                    }),
                "Lazy load failure");

            AssertEqual(0, markOkCount, "Mark OK count");
            AssertEqual(NodeStatus.NeedRun, nodeStatus, "Node status");
        }

        private static void TypeMatcherGeneratedEntriesUseInterfacesWithSingleLeadingI()
        {
            var errors = TypeMatcherUtil.SolidWorksTypes
                .Where(tuple => !tuple.Item2.IsInterface || tuple.Item1 != tuple.Item2.Name.Substring(1))
                .Select(tuple => tuple.Item1 + " => " + tuple.Item2.Name)
                .ToArray();

            if (errors.Length > 0)
                throw new InvalidOperationException("Invalid TypeMatcher entries: " + string.Join("; ", errors));
        }

        private static void ValueArrayInspectionDisplaysAllNullElements()
        {
            var values = new object[] { null, null };
            var lookup = LookupValue.CreateValue(values, typeof(object[]));

            AssertEqual("<NULL>,<NULL>", lookup.ValueName, "All-null array display");
        }

        private static List<List<Segment>> BuildChains(List<Segment> segments)
        {
            return SketchChainTopology.Build(
                segments,
                segment => segment.Start,
                segment => segment.End,
                segment => segment.Reverse(),
                (left, right) => left.ValueEqual(right)).ToList();
        }

        private static Point3D Point(double x, double y)
        {
            return new Point3D(x, y, 0);
        }

        private static void AssertEqual(int expected, int actual, string message)
        {
            if (expected != actual)
                throw new InvalidOperationException(message + ". Expected " + expected + ", got " + actual + ".");
        }

        private static void AssertEqual(bool expected, bool actual, string message)
        {
            if (expected != actual)
                throw new InvalidOperationException(message + ". Expected " + expected + ", got " + actual + ".");
        }

        private static void AssertEqual(string expected, string actual, string message)
        {
            if (expected != actual)
                throw new InvalidOperationException(message + ". Expected " + expected + ", got " + actual + ".");
        }

        private static void AssertEqual(object expected, object actual, string message)
        {
            if (!object.Equals(expected, actual))
                throw new InvalidOperationException(message + ". Expected " + expected + ", got " + actual + ".");
        }

        private static void AssertSame(object expected, object actual, string message)
        {
            if (!ReferenceEquals(expected, actual))
                throw new InvalidOperationException(message + ". Expected same instance.");
        }

        private static void AssertPointEqual(Point3D expected, Point3D actual, string message)
        {
            if (!expected.ValueEqual(actual))
                throw new InvalidOperationException(message + ". Expected " + expected + ", got " + actual + ".");
        }

        private static void AssertClose(double expected, double actual, string message)
        {
            if (Math.Abs(expected - actual) > ExtensionMethods.Eplision)
                throw new InvalidOperationException(message + ". Expected " + expected + ", got " + actual + ".");
        }

        private static TException AssertThrows<TException>(Action action, string message)
            where TException : Exception
        {
            try
            {
                action();
            }
            catch (TException ex)
            {
                return ex;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(message + ". Expected " + typeof(TException).Name + ", got " + ex.GetType().Name + ".");
            }

            throw new InvalidOperationException(message + ". Expected " + typeof(TException).Name + ".");
        }

        private sealed class CurveToken
        {
            public CurveToken(string name)
            {
                Name = name;
            }

            public string Name { get; private set; }
        }

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

        private sealed class SetterOnlyPropertyOwner
        {
            public int WriteOnly
            {
                set { }
            }
        }
    }
}
