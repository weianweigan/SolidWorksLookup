using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Media3D;

namespace SldWorksLookup.PathSplit
{
    public static class SketchChainTopology
    {
        public static List<List<T>> Build<T>(
            IList<T> segments,
            Func<T, Point3D> getStartPoint,
            Func<T, Point3D> getEndPoint,
            Action<T> reverse,
            Func<Point3D, Point3D, bool> pointsEqual)
        {
            if (segments == null)
                throw new ArgumentNullException(nameof(segments));
            if (getStartPoint == null)
                throw new ArgumentNullException(nameof(getStartPoint));
            if (getEndPoint == null)
                throw new ArgumentNullException(nameof(getEndPoint));
            if (reverse == null)
                throw new ArgumentNullException(nameof(reverse));
            if (pointsEqual == null)
                throw new ArgumentNullException(nameof(pointsEqual));

            var chains = new List<List<T>>();
            var remaining = new List<T>(segments);

            while (remaining.Count > 0)
            {
                var firstIndex = FindOpenEndpointSegmentIndex(remaining, getStartPoint, getEndPoint, pointsEqual);
                if (firstIndex < 0)
                    firstIndex = 0;

                var first = remaining[firstIndex];
                var startConnected = IsPointConnected(first, getStartPoint(first), remaining, getStartPoint, getEndPoint, pointsEqual);
                var endConnected = IsPointConnected(first, getEndPoint(first), remaining, getStartPoint, getEndPoint, pointsEqual);

                if (startConnected && !endConnected)
                    reverse(first);

                remaining.RemoveAt(firstIndex);

                var chain = new List<T> { first };
                ConsumeConnectedSegments(chain, remaining, getStartPoint, getEndPoint, reverse, pointsEqual);
                chains.Add(chain);
            }

            return chains;
        }

        private static int FindOpenEndpointSegmentIndex<T>(
            List<T> segments,
            Func<T, Point3D> getStartPoint,
            Func<T, Point3D> getEndPoint,
            Func<Point3D, Point3D, bool> pointsEqual)
        {
            for (var i = 0; i < segments.Count; i++)
            {
                var segment = segments[i];
                var startConnected = IsPointConnected(segment, getStartPoint(segment), segments, getStartPoint, getEndPoint, pointsEqual);
                var endConnected = IsPointConnected(segment, getEndPoint(segment), segments, getStartPoint, getEndPoint, pointsEqual);

                if (!startConnected || !endConnected)
                    return i;
            }

            return -1;
        }

        private static bool IsPointConnected<T>(
            T source,
            Point3D point,
            List<T> segments,
            Func<T, Point3D> getStartPoint,
            Func<T, Point3D> getEndPoint,
            Func<Point3D, Point3D, bool> pointsEqual)
        {
            foreach (var segment in segments)
            {
                if (ReferenceEquals(source, segment))
                    continue;

                if (pointsEqual(point, getStartPoint(segment)) || pointsEqual(point, getEndPoint(segment)))
                    return true;
            }

            return false;
        }

        private static void ConsumeConnectedSegments<T>(
            List<T> chain,
            List<T> remaining,
            Func<T, Point3D> getStartPoint,
            Func<T, Point3D> getEndPoint,
            Action<T> reverse,
            Func<Point3D, Point3D, bool> pointsEqual)
        {
            while (remaining.Count > 0)
            {
                var currentEnd = getEndPoint(chain.Last());
                var nextIndex = -1;
                var reverseNext = false;

                for (var i = 0; i < remaining.Count; i++)
                {
                    if (pointsEqual(currentEnd, getStartPoint(remaining[i])))
                    {
                        nextIndex = i;
                        break;
                    }

                    if (pointsEqual(currentEnd, getEndPoint(remaining[i])))
                    {
                        nextIndex = i;
                        reverseNext = true;
                        break;
                    }
                }

                if (nextIndex < 0)
                    return;

                var next = remaining[nextIndex];
                if (reverseNext)
                    reverse(next);

                remaining.RemoveAt(nextIndex);
                chain.Add(next);
            }
        }
    }
}
