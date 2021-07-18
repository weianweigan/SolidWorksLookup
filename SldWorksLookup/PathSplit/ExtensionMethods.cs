using SolidWorks.Interop.sldworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace SldWorksLookup.PathSplit
{
    public static class ExtensionMethods
    {
        public static double Eplision = 0.00000001;

        public static Point3D ToPoint(this double[] point)
        {
            return new Point3D(point[0], point[1], point[2]);
        }

        public static Point3D ToPoint(this ISketchPoint skePoint)
        {
            return new Point3D(skePoint.X, skePoint.Y, skePoint.Z);
        }

        public static bool ValueEqual(this Point3D point,Point3D anthoer)
        {
            return Math.Abs(point.X - anthoer.X) < Eplision &&
             Math.Abs(point.Y - anthoer.Y) < Eplision &&
             Math.Abs(point.Z - anthoer.Z) < Eplision;
        }

        /// <summary>
        /// 取逆矩阵
        /// </summary>
        /// <param name="transform"></param>
        /// <returns></returns>
        public static Transform3D Inverse(this Transform3D transform)
        {
            var matrix = transform.Value;
            matrix.Invert();
            return new MatrixTransform3D(matrix);
        }
    }
}
