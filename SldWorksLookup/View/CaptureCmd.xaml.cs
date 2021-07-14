using SldWorksLookup.ViewModel;
using System;
using System.Windows;
using System.Windows.Interop;
using Xarial.XCad.SolidWorks;

namespace SldWorksLookup.View
{
    /// <summary>
    /// CaptureCmd.xaml 的交互逻辑
    /// </summary>
    public partial class CaptureCmd : Window
    {
        private readonly SwApplication _app;
        private CaptureCmdViewModel _viewModel;

        public CaptureCmd(SwApplication app)
        {
            this._app = app;
            InitializeComponent();

            _viewModel = new CaptureCmdViewModel(_app);
            _viewModel.CloseAction = new Action(() => this.Close());

            var interopHelper = new WindowInteropHelper(this);
            interopHelper.Owner = _app.WindowHandle;

            this.Closed += CaptureCmd_Closed;

            DataContext = _viewModel;
        }

        private void CaptureCmd_Closed(object sender, EventArgs e)
        {
            this.Closed -= CaptureCmd_Closed;

            _viewModel.DeAttachEvent();
        }
    }
}
