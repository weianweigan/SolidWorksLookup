using Microsoft.VisualBasic;
using SldWorksLookup.PathSplit;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Media3D;

namespace SldWorksLookup.Helper
{
    public static class PathExportUtil
    {
        public static void Export(SolidWorks.Interop.sldworks.ISldWorks sw)
        {
            if (sw == null)
                throw new ArgumentNullException(nameof(sw));

            var modeler = sw.GetModeler() as IModeler;
            if (modeler == null)
                throw new InvalidOperationException("Cannot get SolidWorks modeler.");

            var doc = sw.IActiveDoc2;
            if (doc == null)
                throw new InvalidOperationException("No active document.");

            var selectionManager = doc.ISelectionManager;
            if (selectionManager == null)
                throw new InvalidOperationException("Cannot get selection manager.");

            var feat = selectionManager.GetSelectedObject6(1, -1) as IFeature;
            if (feat == null)
                throw new InvalidOperationException("Select a sketch feature before exporting.");

            var ske = feat.GetSpecificFeature2() as ISketch;
            if (ske == null)
                throw new InvalidOperationException("Selected feature is not a sketch.");

            doc.EditSketch();

            var sketchSegmentArray = ske.GetSketchSegments() as object[];
            if (sketchSegmentArray == null || sketchSegmentArray.Length == 0)
                throw new InvalidOperationException("Selected sketch has no sketch segments.");

            var ses = sketchSegmentArray.Cast<ISketchSegment>().ToList();

            doc.ClearSelection2(true);

            for (int i = 0; i < ses.Count; i++)
            {
                ses[i].Select4(true, null);
            }

            doc.SketchManager.MakeSketchChain();
            doc.ClearSelection2(true);

            var pathArray = ske.GetSketchPaths() as object[];
            if (pathArray == null || pathArray.Length == 0)
                throw new InvalidOperationException("No sketch path was generated.");

            var path = pathArray.Cast<ISketchPath>().FirstOrDefault();
            if (path == null)
                throw new InvalidOperationException("Generated sketch path is invalid.");

            var pathSegmentArray = path.GetSketchSegments() as object[];
            if (pathSegmentArray == null || pathSegmentArray.Length == 0)
                throw new InvalidOperationException("Generated sketch path has no segments.");

            var segs = pathSegmentArray.Cast<ISketchSegment>();

            ICurve curve = null;
            foreach (var seg in segs)
            {
                var seCurve = seg.GetCurve() as ICurve;
                if (seCurve == null)
                    throw new InvalidOperationException("Cannot get sketch segment curve.");

                var wrapper = new SketchSegmentWrapper(seg);
                var sp = wrapper.SourceStartPoint;
                var ep = wrapper.SourceEndPoint;

                seCurve = seCurve.CreateTrimmedCurve2(sp.X, sp.Y, sp.Z, ep.X, ep.Y, ep.Z);

                var body = seCurve.CreateWireBody();
                body.Display2(doc as PartDoc, Information.RGB(255, 0, 0), (int)swTempBodySelectOptions_e.swTempBodySelectOptionNone);

                curve = curve == null
                    ? seCurve
                    : modeler.MergeCurves(new object[] { curve, seCurve });
            }

            if (curve == null)
                throw new InvalidOperationException("Cannot create a merged curve from the generated sketch path.");

            doc.InsertSketch();

            var points = SplitCurve(curve, 10);

            doc.Insert3DSketch();
            var ske3D = doc.SketchManager.ActiveSketch;

            foreach (var point in points)
            {
                doc.SketchManager.CreatePoint(point.X, point.Y, point.Z);
            }
        }

        public static List<Point3D> SplitCurve(ICurve curve, int num)
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
