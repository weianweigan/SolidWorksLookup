using SldWorksLookup.ViewModel;
using System.Windows;

namespace SldWorksLookup.View
{
    /// <summary>
    /// ColorToIntWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ColorToIntWindow : Window
    {
        public ColorToIntWindow()
        {
            Xceed.Wpf.Toolkit.IconButton icBtn = new Xceed.Wpf.Toolkit.IconButton();

            InitializeComponent();

            DataContext = new ColorToIntWindowViewModel();
        }
    }
}
