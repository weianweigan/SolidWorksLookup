using System.Windows;

namespace SldWorksLookup.View
{
    /// <summary>
    /// SelectionPIDWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SelectionPIDWindow : Window
    {
        private SelectionPIDViewModel _vM;

        public SelectionPIDWindow()
        {
            InitializeComponent();
            _closeButton.Click += (sender, e) =>  this.Close();
        }

        public SelectionPIDViewModel VM
        {
            get => _vM; set
            {
                _vM = value;
                DataContext = _vM;
            }
        }

        private void ListBox_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _vM?.DoubleClick(sender, e);
        }
    }
}
