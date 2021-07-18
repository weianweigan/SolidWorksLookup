using SolidWorks.Interop.sldworks;
using System.Windows;
using System.Windows.Interop;

namespace SldWorksLookup.PathSplit
{
    /// <summary>
    /// PathSplitWindow.xaml 的交互逻辑
    /// </summary>
    public partial class PathSplitWindow : Window
    {
        public PathSplitWindow(ModelDoc2 doc, System.IntPtr parent)
        {
            InitializeComponent();

            //设置SolidWorks为父窗口
            var windowHelper = new WindowInteropHelper(this);
            windowHelper.Owner = parent;

            DataContext = new PathSplitWindowViewModel(doc);
        }
    }
}
