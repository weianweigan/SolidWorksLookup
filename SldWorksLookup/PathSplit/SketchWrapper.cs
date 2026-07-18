using SolidWorks.Interop.sldworks;
using System.Collections.Generic;
using System.Linq;

namespace SldWorksLookup.PathSplit
{
    public class SketchWrapper
    {
        #region Fields
        private readonly ISketch _sketch;
        private IFeature _feat;
        private readonly IComponent2 _comp;
        #endregion

        #region Ctor

        public SketchWrapper(IFeature feat)
        {
            _feat = feat;
            _sketch = _feat.GetSpecificFeature2() as ISketch;
        }

        public SketchWrapper(IFeature feat, IComponent2 comp)
        {
            _feat = feat;
            _comp = comp;
            _sketch = _feat.GetSpecificFeature2() as ISketch;
        }
        #endregion

        #region Public Methods
        public IEnumerable<SketchChain> GetChains()
        {
            var sketchSegments = _sketch.GetSketchSegments() as object[];
            if (sketchSegments == null)
                yield break;

            var ses = sketchSegments
                .Cast<ISketchSegment>()
                .Select(p => new SketchSegmentWrapper(p))
                .ToList();

            var chains = SketchChainTopology.Build(
                ses,
                segment => segment.StartPoint,
                segment => segment.EndPoint,
                segment => segment.ReverseSpAndEp = !segment.ReverseSpAndEp,
                (left, right) => left.ValueEqual(right));

            foreach (var chain in chains)
            {
                yield return new SketchChain(_sketch, chain, _comp);
            }
        }

        public override string ToString()
        {
            return (_feat?.Name + _comp?.Name2 )?? base.ToString();
        }
        #endregion

    }
}
