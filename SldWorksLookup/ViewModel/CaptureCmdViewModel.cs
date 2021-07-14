using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using SldWorksLookup.Model;
using SolidWorks.Interop.sldworks;
using System;
using System.Collections.ObjectModel;
using Xarial.XCad.SolidWorks;

namespace SldWorksLookup.ViewModel
{

    public class CaptureCmdViewModel : ViewModelBase
    {
        private SwApplication _app;
        private SldWorks _sw;
        private RelayCommand _closeCommand;

        public CaptureCmdViewModel(SwApplication app)
        {
            this._app = app;

            _sw = _app.Sw as SldWorks;

            _sw.CommandOpenPreNotify += _sw_CommandOpenPreNotify;
        }

        public ObservableCollection<SwCommand> Commands { get; set; } = new ObservableCollection<SwCommand>();
        
        public Action CloseAction { get; internal set; }

        public RelayCommand CloseCommand { get => _closeCommand ?? (_closeCommand= new RelayCommand(() => CloseAction?.Invoke()));}

        private int _sw_CommandOpenPreNotify(int Command, int UserCommand)
        {
            Commands.Add(new SwCommand(Command, UserCommand));
            return 0;
        }

        internal void DeAttachEvent()
        {
            _sw.CommandOpenPreNotify -= _sw_CommandOpenPreNotify;
        }
    }
}
