using System;

namespace SldWorksLookup.PathSplit
{
    public sealed class SegmentSamplingPlan
    {
        private SegmentSamplingPlan(int pointCount, double firstDistance, double distanceToNextPoint)
        {
            PointCount = pointCount;
            FirstDistance = firstDistance;
            DistanceToNextPoint = distanceToNextPoint;
        }

        public int PointCount { get; }

        public double FirstDistance { get; }

        public double DistanceToNextPoint { get; }

        public static SegmentSamplingPlan Create(double segmentLength, double stepLength, double distanceToNextPoint)
        {
            if (stepLength <= 0 || double.IsNaN(stepLength) || double.IsInfinity(stepLength))
                throw new ArgumentOutOfRangeException(nameof(stepLength));
            if (segmentLength < 0 || double.IsNaN(segmentLength) || double.IsInfinity(segmentLength))
                throw new ArgumentOutOfRangeException(nameof(segmentLength));
            if (distanceToNextPoint < 0 || double.IsNaN(distanceToNextPoint) || double.IsInfinity(distanceToNextPoint))
                throw new ArgumentOutOfRangeException(nameof(distanceToNextPoint));

            if (distanceToNextPoint > segmentLength)
                return new SegmentSamplingPlan(0, 0, distanceToNextPoint - segmentLength);

            var count = (int)Math.Floor((segmentLength - distanceToNextPoint) / stepLength) + 1;
            var lastDistance = distanceToNextPoint + (count - 1) * stepLength;
            var nextDistance = stepLength - (segmentLength - lastDistance);
            return new SegmentSamplingPlan(count, distanceToNextPoint, nextDistance);
        }
    }
}
