using SldWorksLookup.Model;
using SldWorksLookup.ViewModel;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using Xarial.XCad.SolidWorks;

namespace SldWorksLookup.View
{
    /// <summary>
    /// LookupPropertyWindow.xaml 的交互逻辑
    /// </summary>
    public partial class LookupPropertyWindow : Window
    {
        private BackgroundWorker _worker = new BackgroundWorker();

        public LookupWindowViewModel VM { get; private set; }

        #region Ctor

        public LookupPropertyWindow()
        {
            Xceed.Wpf.Toolkit.AutoSelectTextBox autoS = new Xceed.Wpf.Toolkit.AutoSelectTextBox();
            InitializeComponent();
        }

        public LookupPropertyWindow(InstanceProperty instanceProperty)
        {
            Xceed.Wpf.Toolkit.AutoSelectTextBox autoS = new Xceed.Wpf.Toolkit.AutoSelectTextBox();
            InitializeComponent();

            VM = new LookupWindowViewModel(instanceProperty);

            BindingAndInit();
        }

        public LookupPropertyWindow(IList<InstanceProperty> instanceProperties)
        {
            MutiInit(instanceProperties);
        }

        public void ISldWorksInit(SwApplication swApplication)
        {
            VM = new LookupWindowViewModel(swApplication);

            BindingAndInit();
        }

        #endregion

        public void MutiInit(IList<InstanceProperty> instanceProperties)
        {
            Xceed.Wpf.Toolkit.AutoSelectTextBox autoS = new Xceed.Wpf.Toolkit.AutoSelectTextBox();
            InitializeComponent();

            VM = new LookupWindowViewModel(instanceProperties);

            BindingAndInit();
        }

        private void BindingAndInit()
        {
            VM.ExitOrUpdate += ExitOrUpdateWindow;
            DataContext = VM;
            _worker.DoWork += _worker_DoWork;
            _worker.RunWorkerCompleted += _worker_RunWorkerCompleted;
        }    

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ExitOrUpdateWindow(int messgae)
        {
            if (messgae == 0)
            {
                this.Close();
            }
            else if (messgae == 1)
            {
                //ReGrid
                var properties = VM.Properties;
                VM.Properties = null;
                VM.Properties = properties;
            }
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (sender is TreeView treeView)
            {
                var insTree = treeView.SelectedItem as InstanceTree;

                if (insTree != null)
                {
                    //后台运行不能提高速度，卡顿因为属性过多，控件重绘过程中。
                    //即使不堵塞UI，也会有卡顿；后台运行还有可能对COM组件产生影响。
                    //if (_worker.IsBusy)
                    //{
                    //    MessageBox.Show("Waiting...");
                    //    return;
                    //}
                    //_worker.RunWorkerAsync(insTree);

                    insTree.ResolveProperties();
                    VM.SelectedTreeItem = insTree;
                }
            }
        }

        private void _worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            var insTree = e.Result as InstanceTree;
            if (insTree != null)
            {
                VM.SelectedTreeItem = insTree;
            }
        }

        private void _worker_DoWork(object sender, DoWorkEventArgs e)
        {
            var insTree = e.Argument as InstanceTree;
            insTree.ResolveProperties();
            e.Result = insTree;
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Hyperlink link)
            {
                Process.Start(new ProcessStartInfo(link.NavigateUri.AbsoluteUri));
            }
        }
    }
}
