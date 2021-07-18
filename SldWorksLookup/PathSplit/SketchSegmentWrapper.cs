using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Media3D;

namespace SldWorksLookup.PathSplit
{
    /// <summary>
    /// 草图图元封装，一边明确此图元在 <see cref="SketchChain"/> 中的起始点
    /// </summary>
    public class SketchSegmentWrapper
    {
        #region Fields
        private double? _length;
        #endregion

        #region Ctor
        public SketchSegmentWrapper(ISketchSegment segment)
        {
            Segment = segment;

            //获取绘制时的起点和中点
            GetSpAndEp(segment, out var sp, out var ep);
            SourceStartPoint = sp;
            SourceEndPoint = ep;
        }
        #endregion

        #region Properties
        /// <summary>
        /// 原始起点，即用户绘制时的起点
        /// </summary>
        public Point3D SourceStartPoint { get; }

        /// <summary>
        /// 原始终点，即用户绘制时的终点
        /// </summary>
        public Point3D SourceEndPoint { get; }

        /// <summary>
        /// 是否需要反转起点和终点
        /// </summary>
        public bool ReverseSpAndEp { get; set; }

        /// <summary>
        /// 在链中的起点
        /// </summary>
        public Point3D StartPoint => ReverseSpAndEp ? SourceEndPoint : SourceStartPoint;

        /// <summary>
        /// 在链中的终点
        /// </summary>
        public Point3D EndPoint => ReverseSpAndEp ? SourceStartPoint : SourceEndPoint;

        /// <summary>
        /// SolidWork中的草图图元
        /// </summary>
        public ISketchSegment Segment { get; }

        #endregion

        #region Public Methods

        /// <summary>
        /// 获取长度
        /// </summary>
        /// <returns></returns>
        public double GetLength()
        {
            if (_length != null)
                return _length.Value;
            else
                return (_length = Segment.GetLength()).Value;
        }

        /// <summary>
        /// 当前起点是否和这个<paramref name="segment"/>相连
        /// </summary>
        /// <param name="segment">另外一个草图段</param>
        /// <returns>是否相连</returns>
        public bool IsStartConnected(SketchSegmentWrapper segment)
        {
            return (StartPoint.ValueEqual(segment.StartPoint) || StartPoint.ValueEqual(segment.EndPoint));
        }

        /// <summary>
        /// 当前终点是否和这个<paramref name="segment"/>相连
        /// </summary>
        /// <param name="segment">另外一个草图段</param>
        /// <returns>是否相连</returns>
        public bool IsEndConnected(SketchSegmentWrapper segment)
        {
            return (EndPoint.ValueEqual(segment.StartPoint) || EndPoint.ValueEqual(segment.EndPoint));
        }

        /// <summary>
        /// 分割
        /// </summary>
        /// <param name="stepLength">步长</param>
        /// <param name="spareLength">上一段多余的长度</param>
        /// <param name="newSpareLength">这一段剩余的长度</param>
        /// <returns>分割后的点</returns>
        public List<Point3D> SplitSegment(double stepLength, double spareLength ,out double newSpareLength)
        {
            if (stepLength > GetLength())
            {
                throw new InvalidOperationException($"步长：{stepLength} 大于 当前草图元素长度");
            }

            //获取草图线段曲线
            var seCurve = Segment.GetCurve() as ICurve;

            //裁剪曲线
            seCurve = seCurve.CreateTrimmedCurve2(SourceStartPoint.X, SourceStartPoint.Y, SourceStartPoint.Z, SourceEndPoint.X, SourceEndPoint.Y, SourceEndPoint.Z);

            var points = SplitCurve(seCurve, stepLength, spareLength, out newSpareLength);

            return points;
        }

        /// <summary>
        /// 分割曲线
        /// </summary>
        /// <param name="stepLength">步长</param>
        /// <param name="spareLength">上一段多余的长度</param>
        /// <param name="newSpareLength">这一段剩余的长度</param>
        /// <returns>分割后的点</returns>
        private List<Point3D> SplitCurve(ICurve curve, double stepLength, double spareLength, out double newSpareLength)
        {
            //初始化点几何
            var points = new List<Point3D>();

            //获取曲线元素
            curve.GetEndParams(out var startParam, out var endParam, out bool isClosed, out bool isPeriodic);

            if (ReverseSpAndEp)
            {
                //数量，取整了
                int num = (int)((GetLength() - spareLength )/ stepLength);

                //步长参数
                var incr = (endParam - startParam) / (num);

                for (int i = 0; i < num; i++)
                {
                    var param = curve.Evaluate(startParam + i * incr) as double[];
                    points.Add(new Point3D(param[0], param[1], param[2]));
                }

                if (spareLength < ExtensionMethods.Eplision)
                {
                    //第一个点参数
                    var lastParam = endParam - ((endParam - startParam) / (GetLength() / spareLength));
                    var lastPoint = curve.Evaluate(lastParam) as double[];
                    points.Add(lastPoint.ToPoint());
                }

                //逆序
                points.Reverse();

                newSpareLength = GetLength() - spareLength - stepLength * num;
            }
            else
            {
                //第一个点参数
                var firstParam = ((endParam - startParam) / (GetLength() / spareLength)) + startParam;

                //数量，取整了
                int num = (int)(GetLength() / stepLength);

                //步长参数
                var incr = (endParam - firstParam) / (num);

                for (int i = 0; i < num; i++)
                {
                    var param = curve.Evaluate(firstParam + i * incr) as double[];
                    points.Add(new Point3D(param[0], param[1], param[2]));
                }

                newSpareLength = GetLength() - spareLength - stepLength * num;
            }

            if (newSpareLength < stepLength)
            {
                newSpareLength = stepLength - newSpareLength;
            }

            return points;
        }

        private List<Point3D> SplitCurve(ICurve curve, int num)
        {
            var points = new List<Point3D>();

            curve.GetEndParams(out var startParam, out var endParam, out bool isClosed, out bool isPeriodic);

            var incr = (endParam - startParam) / (num - 1);

            for (int i = 0; i < num; i++)
            {
                var param = curve.Evaluate(startParam + i * incr) as double[];
                points.Add(new Point3D(param[0], param[1], param[2]));
            }

            return points;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// 根据图元类型获取其起点和终点
        /// </summary>
        /// <param name="seg"></param>
        /// <param name="sp">起点</param>
        /// <param name="ep">终点</param>
        /// <exception cref="NotSupportedException">不支持<see cref="swSketchSegments_e.swSketchTEXT"/></exception>
        private static void GetSpAndEp(ISketchSegment seg, out Point3D sp, out Point3D ep)
        {
            sp = default; ep = default;
            switch ((swSketchSegments_e)seg.GetType())
            {
                case swSketchSegments_e.swSketchLINE:
                    var line = seg as ISketchLine;
                    sp = (line.GetStartPoint2() as ISketchPoint).ToPoint();
                    ep = (line.GetEndPoint2() as ISketchPoint).ToPoint();
                    break;
                case swSketchSegments_e.swSketchARC:
                    var arc = seg as ISketchArc;
                    sp = (arc.GetStartPoint2() as ISketchPoint).ToPoint();
                    ep = (arc.GetEndPoint2() as ISketchPoint).ToPoint();
                    break;
                case swSketchSegments_e.swSketchELLIPSE:
                    var eli = seg as ISketchEllipse;
                    sp = (eli.GetStartPoint2() as ISketchPoint).ToPoint();
                    ep = (eli.GetEndPoint2() as ISketchPoint).ToPoint();
                    break;
                case swSketchSegments_e.swSketchSPLINE:
                    var spline = seg as SketchSpline;
                    var points = (spline.GetPoints2() as object[]).Cast<SketchPoint>().ToList();
                    sp = new Point3D(points[0].X, points[0].Y, points[0].Z);
                    ep = new Point3D(points[points.Count - 1].X, points[points.Count - 1].Y, points[points.Count - 1].Z);
                    break;
                case swSketchSegments_e.swSketchTEXT:
                    throw new NotSupportedException("swSketchTEXT");
                case swSketchSegments_e.swSketchPARABOLA:
                    var para = seg as SketchParabola;
                    sp = (para.GetStartPoint2() as ISketchPoint).ToPoint();
                    ep = (para.GetEndPoint2() as ISketchPoint).ToPoint();
                    break;
                default:
                    throw new NotSupportedException();
            }
        }

        #endregion
    }
}
