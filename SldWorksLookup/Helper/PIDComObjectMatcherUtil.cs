using SolidWorks.Interop.sldworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SldWorksLookup
{
    public static class PIDComObjectMatcherUtil
    {
        public static readonly Type[] MatchTypes = new Type[]
        {
            typeof(IBody2),typeof(IFace2),typeof(ILoop2),typeof(IEdge),typeof(IVertex),
            typeof(IFeature),typeof(IConfiguration),typeof(IModelDoc2),typeof(IComponent2),
            typeof(ISketchLine),typeof(ISketchArc),typeof(ISketchEllipse),typeof(SketchSlot),typeof(ISketchPoint),typeof(ISketchSegment)
        };

        public static Type Match(object obj)
        {
            var desType = MatchTypes.Where(p => p.IsInstanceOfType(obj)).FirstOrDefault();

            return desType;
        }

        public static string GetID(object obj)
        {
            string id = string.Empty;

            if (obj is IComponent2 comp)
            {
                id = comp.GetID().ToString();
            }
            else if(obj is IConfiguration config)
            {
                id = config.GetID().ToString();
            }
            else if(obj is IFeature feat)
            {
                id = feat.GetID().ToString();
            }
            else if(obj is ILayer layer)
            {
                id = layer.GetID().ToString();
            }
            else if (obj is ILight light)
            {
                id = light.GetID().ToString();
            }
            else if (obj is ISketchPoint point)
            {
                id = ArrayIDConvert(point.GetID() as int[]);
            }
            else if (obj is ISheet sheet)
            {
                id = sheet.GetID().ToString();
            }
            else if (obj is ISketchHatch hatch)
            {
                id = ArrayIDConvert(hatch.GetID() as int[]);
            }
            else if (obj is ISketchSegment segment)
            {
                id = ArrayIDConvert(segment.GetID() as int[]);
            }

            return id;
        }

        public static string GetName(object obj,IModelDoc2 doc)
        {
            string name = string.Empty;

            if (obj is IComponent2 comp)
            {
                name = comp.Name2;
            }
            else if (obj is IConfiguration config)
            {
                name = config.Name;
            }
            else if (obj is IFeature feat)
            {
                name = feat.Name;
            }
            else if (obj is ILayer layer)
            {
                name = layer.Name;
            }
            else if (obj is ISheet sheet)
            {
                name = sheet.GetName();
            }
            else if (obj is IBody2 body)
            {
                name = body.Name;
            }
            else if (obj is IFace2 
                || obj is IEdge 
                || obj is IVertex 
                || obj is Entity)
            {
                name = doc.GetEntityName(obj);
            }
            else if (obj is IView view)
            {
                name = view.Name;
            }

            return name;
        }

        private static string ArrayIDConvert(int[] ids)
        {
            if (ids == null || ids.Length == 0)
            {
                return string.Empty;
            }
            if (ids.Length == 1)
            {
                return ids.Single().ToString();
            }
            return ids.Select(p => p.ToString()).Aggregate((p1, p2) => $"{p1} {p2}");
        }
    }
}
