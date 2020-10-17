using System.Windows;

namespace SldWorksLookup.View
{
    /// <summary>
    /// GetObjectByPIDWindow.xaml 的交互逻辑
    /// </summary>
    public partial class GetObjectByPIDWindow : Window
    {
        private GetObjectByPIDWindowViewModel _vM;

        public GetObjectByPIDWindow()
        {
            InitializeComponent();
        }

        public GetObjectByPIDWindowViewModel VM
        {
            get => _vM; internal set
            {
                _vM = value;
                DataContext = _vM;
            }
        }
    }
}
