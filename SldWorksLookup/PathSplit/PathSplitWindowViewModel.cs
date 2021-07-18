using GalaSoft.MvvmLight;
using SolidWorks.Interop.sldworks;
using System.Collections.ObjectModel;

namespace SldWorksLookup.PathSplit
{
    public class PathSplitWindowViewModel : ViewModelBase
    {
        #region Fields
        private readonly IModelDoc2 _doc;
        private ModelPathsSolver _solver;
        private ObservableCollection<SketchWrapper> _sketchs;
        private SketchWrapper _selectedSketch;
        private ObservableCollection<SketchChain> _sketchChains;
        private SketchChain _selectedSketchChain;
        #endregion

        #region Ctor
        public PathSplitWindowViewModel(IModelDoc2 doc)
        {
            _solver = new ModelPathsSolver(doc);

            Sketchs = new ObservableCollection<SketchWrapper>(_solver.GetSketchWrappers());
            _doc = doc;
        }
        #endregion

        #region Properties

        /// <summary>
        /// 所有草图
        /// </summary>
        public ObservableCollection<SketchWrapper> Sketchs { get => _sketchs; set => Set(ref _sketchs, value); }
        
        /// <summary>
        /// 用户选中的草图
        /// </summary>
        public SketchWrapper SelectedSketch
        {
            get => _selectedSketch; set
            {
                Set(ref _selectedSketch, value);
                if (value != null)
                {
                    SketchChains = new ObservableCollection<SketchChain>(SelectedSketch.GetChains());
                }
            }
        }

        /// <summary>
        /// 当前选中草图中包含的路径（Chain）
        /// </summary>
        public ObservableCollection<SketchChain> SketchChains { get => _sketchChains; set => Set(ref _sketchChains, value); }

        /// <summary>
        /// 当前用户选择的枯竭
        /// </summary>
        public SketchChain SelectedSketchChain
        {
            get => _selectedSketchChain; set
            {
                Set(ref _selectedSketchChain, value);
                if (_selectedSketchChain != null)
                {
                    //在图形区域高亮
                    _doc.ClearSelection2(true);
                    _selectedSketchChain.Select();
                }
            }
        }
        #endregion

    }
}
