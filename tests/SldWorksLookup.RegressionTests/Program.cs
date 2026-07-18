using SldWorksLookup.PathSplit;
using System;
using System.Collections.Generic;
using System.Linq;
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
                SamplingPlanCarriesSpacingAcrossShortSegment,
                SamplingPlanRejectsNonPositiveOrNonFiniteStep
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

        private static void SamplingPlanRejectsNonPositiveOrNonFiniteStep()
        {
            AssertThrows<ArgumentOutOfRangeException>(() => SegmentSamplingPlan.Create(1.0, 0, 0), "Zero step");
            AssertThrows<ArgumentOutOfRangeException>(() => SegmentSamplingPlan.Create(1.0, -1.0, 0), "Negative step");
            AssertThrows<ArgumentOutOfRangeException>(() => SegmentSamplingPlan.Create(1.0, double.NaN, 0), "NaN step");
            AssertThrows<ArgumentOutOfRangeException>(() => SegmentSamplingPlan.Create(1.0, double.PositiveInfinity, 0), "Infinite step");
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

        private static void AssertThrows<TException>(Action action, string message)
            where TException : Exception
        {
            try
            {
                action();
            }
            catch (TException)
            {
                return;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(message + ". Expected " + typeof(TException).Name + ", got " + ex.GetType().Name + ".");
            }

            throw new InvalidOperationException(message + ". Expected " + typeof(TException).Name + ".");
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
    }
}
