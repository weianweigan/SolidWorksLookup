using GalaSoft.MvvmLight.Command;
using SolidWorks.Interop.sldworks;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Media3D;

namespace SldWorksLookup.PathSplit
{
    /// <summary>
    /// 一个草图路径
    /// </summary>
    public class SketchChain
    {
        #region Fields
        private RelayCommand _exportCommand;
        private readonly ISketch _sketch;
        private readonly IComponent2 _comp;
        #endregion

        #region Ctor
        public SketchChain(ISketch sketch,List<SketchSegmentWrapper> segs,IComponent2 comp = null)
        {
            _sketch = sketch;
            Segs = segs;
            _comp = comp;
        }
        #endregion

        #region Properties
        public string Name => ToString();
        
        public string Length => $"长度：{GetLength()}";
        
        /// <summary>
        /// 所有草图图元
        /// </summary>
        public List<SketchSegmentWrapper> Segs { get; }

        /// <summary>
        /// 细分步长
        /// </summary>
        public double StepLength { get; set; } = 0.00002;

        /// <summary>
        /// 导出命令
        /// </summary>
        public RelayCommand ExportCommand { get => _exportCommand ?? (_exportCommand = new RelayCommand(ExportClick)); }
        #endregion

        #region Public Methods
        /// <summary>
        /// 获取长度，懒加载，记录在字段中
        /// </summary>
        /// <returns></returns>
        public double GetLength()
        {
            return Segs.Count == 1 ? (Segs.FirstOrDefault()?.GetLength() ?? 0) : Segs.Select(p => p.GetLength()).Aggregate((l1, l2) => l1 +l2 );
        }

        /// <summary>
        /// 按步长分割
        /// </summary>
        /// <param name="stepLength"></param>
        /// <returns></returns>
        public List<Point3D> Split(double stepLength)
        {
            //初始化列表
            List<Point3D> points = new List<Point3D>();

            //按段细分
            double spareLength = 0;
            foreach (var seg in Segs)
            {
                var length = seg.Segment.GetLength();

                //是否有上根线段的剩余长度
                var curvePts = seg.SplitSegment(stepLength, spareLength,out var nextSpareLength);

                spareLength = nextSpareLength;

                points.AddRange(curvePts);
;           }

            //二维草图需要变换坐标
            if (!_sketch.Is3D())
            {
                var trans = _sketch.ModelToSketchTransform.ToGeneralTransform3D().Inverse();
                points = points.Select(p => trans.Transform(p)).ToList();
            }

            //从零件空间变换到装配体空间
            if (_comp != null)
            {
                var trans = _comp.Transform2.ToGeneralTransform3D();
                points = points.Select(p => trans.Transform(p)).ToList();
            }

            return points;
        }

        /// <summary>
        /// 选中当前路径
        /// </summary>
        public void Select()
        {
            foreach (var seg in Segs)
            {
                seg?.Segment?.Select2(true, 0);
            }
        }

        public override string ToString()
        {
            return $"Path,草图图元数量：{Segs?.Count ?? 0}";
        }

        #endregion

        #region Private Methods
        /// <summary>
        /// 用户点击导出按钮执行的方法
        /// </summary>
        public void ExportClick()
        {
            //获取点
            var points = Split(StepLength);

            //显示
            var window = new Window()
            {
                Content = new ListBox()
                {
                    ItemsSource = points
                },
                Width = 400,
                Height = 500,
                Title = $"{points.Count}个"
            };

            window.ShowDialog();
        }
        #endregion
    }
}
