/*
 * Created By WeiGan 2021/3/20
 * 
 * 1.添加MathTransform变换为矩阵matrix3d的方法
 * 
 */

using System.Windows.Media.Media3D;

namespace SolidWorks.Interop.sldworks
{
    public static class IMathTransformExtension
    {
        public static Transform3D ToGeneralTransform3D(this IMathTransform transform)
        {
            MatrixTransform3D matrixTransform = new MatrixTransform3D(transform.ToMatrix3D());
            return matrixTransform;
        }

        /// <summary>
        ///    |a b c.n |
        ///    |d e f.o |
        ///    |g h i.p |  
        ///    |j k l.m |
        ///    变换为 <see cref="Matrix3D"/>
        /// </summary>
        /// <remarks>
        /// 三个轴必须是正交和单位化的以便进行正交变换。通过把组件设置为负的也可以添加仿射变换。
        /// 
        /// 如果三个轴被一个子矩阵设置为非正交或者非单位化，矩阵将按照一下规则将其修正为单位正交矩阵：
        ///   
        /// 1.如果轴是0，或者两个轴平行了，再或者所有轴共面了，子矩阵将被一个单位矩阵代替。
        /// 2.所有轴会被单位化。
        /// 3.轴之间相互正交，顺序如下：Z X Y (X正交于Z，Y正交于Z和X)
        /// 
        /// </remarks>
        /// <returns></returns>
        public static Matrix3D ToMatrix3D(this IMathTransform transform)
        {
            var data = transform.ArrayData as double[];
            var matrix = new Matrix3D(
                data[0], data[1], data[2], data[13],
                data[3], data[4], data[5], data[14],
                data[6], data[7], data[8], data[15],
                data[9], data[10], data[11], data[12]
                );
            return matrix;
        }
    }
}
