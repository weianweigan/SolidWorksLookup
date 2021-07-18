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
        #endregion

        #region Ctor
        public SketchWrapper(IFeature feat)
        {
            _feat = feat;
            _sketch = _feat.GetSpecificFeature2() as ISketch;
        }
        #endregion

        #region Public Methods
        public IEnumerable<SketchChain> GetChains()
        {
            var ses = (_sketch.GetSketchSegments() as object[])
                .Cast<ISketchSegment>()
                .Select(p => new SketchSegmentWrapper(p))
                .ToList();

            //挑选出起点
            for (int i = 0; i < ses.Count; i++)
            {
                //判断当前直线是否可以作为路径其实直线
                bool startConnected = false;
                bool endConnected = false;
                for (int j = 0; j < ses.Count; j++)
                {
                    if (i == j)
                        continue;

                    if(!startConnected)
                        startConnected = ses[i].IsStartConnected(ses[j]);
                    if (!endConnected)
                        endConnected = ses[j].IsEndConnected(ses[j]);

                    if (startConnected && endConnected)
                        break;
                }

                if (!startConnected || !endConnected)
                {
                    //起点和其他直线相连，交换起点和终点
                    if (startConnected)
                        ses[i].ReverseSpAndEp = true;

                    var chainSes = new List<SketchSegmentWrapper>() { ses[i] };
                    ses.RemoveAt(i);

                    //构建新列表
                    var newSes = new List<SketchSegmentWrapper>();
                    newSes.AddRange(ses);

                    //寻找其他相连直线
                    var next = chainSes.Last();

                    //递归查找
                    while (next != null)
                    {
                        next = SearchNext(chainSes, next, newSes);
                    }

                    //从现有列表中剔除
                    foreach (var usedSe in chainSes)
                    {
                        ses.Remove(usedSe);
                    }

                    //重新查找
                    i = 0;

                    //返回草图链条
                    yield return new SketchChain(_sketch,chainSes);
                }
            }
        }

        public override string ToString()
        {
            return _feat?.Name ?? base.ToString();
        }
        #endregion

        #region Private Methods
        private static SketchSegmentWrapper SearchNext(List<SketchSegmentWrapper> chainSes, SketchSegmentWrapper current, List<SketchSegmentWrapper> newSes)
        {
            var next = default(SketchSegmentWrapper);
            for (int k = 0; k < newSes.Count; k++)
            {
                if (current.EndPoint.ValueEqual(newSes[k].StartPoint))
                {
                    chainSes.Add(newSes[k]);
                    newSes.RemoveAt(k--);
                    next = chainSes.Last();
                }
                else if (current.EndPoint.ValueEqual(newSes[k].EndPoint))
                {
                    newSes[k].ReverseSpAndEp = true;
                    chainSes.Add(newSes[k]);
                    newSes.RemoveAt(k--);
                    next = chainSes.Last();
                }
                if (next != null)
                {
                    break;
                }
            }

            return next;
        }
        #endregion

    }
}
