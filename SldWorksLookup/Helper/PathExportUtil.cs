using Microsoft.VisualBasic;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace SldWorksLookup.Helper
{
    public static class PathExportUtil
    {
        public static void Export(SolidWorks.Interop.sldworks.ISldWorks sw)
        {
            var modeler = sw.GetModeler() as IModeler;

            var doc = sw.IActiveDoc2;

            var feat = doc.ISelectionManager.GetSelectedObject6(1, -1) as IFeature;

            var ske = feat.GetSpecificFeature2() as ISketch;

            doc.EditSketch();

            var ses = (ske.GetSketchSegments() as object[]).Cast<ISketchSegment>().ToList();

            doc.ClearSelection2(true);

            for (int i = 0; i < ses.Count; i++)
            {
                ses[i].Select4(true, null);
            }

            doc.SketchManager.MakeSketchChain();
            doc.ClearSelection2(true);

            var path = (ske.GetSketchPaths() as object[]).Cast<ISketchPath>().First();
            
            var segs = (path.GetSketchSegments() as object[]).Cast<ISketchSegment>();

            ICurve curve = null;
            foreach (var seg in segs)
            {
                var seCurve = seg.GetCurve() as ICurve;

                //剪裁曲线
                GetSpAndEp(seg,out Point3D sp,out Point3D ep);

                seCurve = seCurve.CreateTrimmedCurve2(sp.X, sp.Y, sp.Z, ep.X, ep.Y, ep.Z);

                var body = seCurve.CreateWireBody();
                body.Display2(doc as PartDoc, Information.RGB(255, 0, 0), (int)swTempBodySelectOptions_e.swTempBodySelectOptionNone);

                if (curve == null)
                {
                    curve = seCurve;
                }
                else
                {
                    curve = modeler.MergeCurves(new object[] { curve, seCurve });
                }
            }

            doc.InsertSketch();

            var points = SplitCurve(curve,10); 

            doc.Insert3DSketch();
            var ske3D = doc.SketchManager.ActiveSketch;

            foreach (var point in points)
            {
                doc.SketchManager.CreatePoint(point.X,point.Y,point.Z);

            }
        }

        private static void GetSpAndEp(ISketchSegment seg, out Point3D sp, out Point3D ep)
        {
            sp = default;ep = default;
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
                    ep = (arc.GetStartPoint2() as ISketchPoint).ToPoint();
                    break;
                case swSketchSegments_e.swSketchELLIPSE:
                    var eli= seg as ISketchEllipse;
                    sp = (eli.GetStartPoint2() as ISketchPoint).ToPoint();
                    ep = (eli.GetStartPoint2() as ISketchPoint).ToPoint();
                    break;
                case swSketchSegments_e.swSketchSPLINE:
                    var spline = seg as SketchSpline;
                    var points = (spline.GetPoints2() as object[]).Cast<SketchPoint>().ToList();
                    sp = new Point3D(points[0].X, points[0].Y, points[0].Z);
                    ep = new Point3D(points[points.Count-1].X, points[points.Count - 1].Y, points[points.Count - 1].Z);
                    break;
                case swSketchSegments_e.swSketchTEXT:
                    throw new NotSupportedException();
                case swSketchSegments_e.swSketchPARABOLA:
                    var para = seg as SketchParabola;
                    sp = (para.GetStartPoint2() as ISketchPoint).ToPoint();
                    ep = (para.GetStartPoint2() as ISketchPoint).ToPoint();
                    break;
                default:
                    throw new NotSupportedException();
            }
        }

        public static Point3D ToPoint(this double[] point)
        {
            return new Point3D(point[0], point[1], point[2]);
        }

        public static Point3D ToPoint(this ISketchPoint skePoint)
        {
            return new Point3D(skePoint.X, skePoint.Y, skePoint.Z);
        }

        public static List<Point3D> SplitCurve(ICurve curve,int num)
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
    }

}
