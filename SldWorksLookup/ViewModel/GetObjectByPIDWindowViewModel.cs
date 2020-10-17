using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using SldWorksLookup.Model;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using System;
using Xarial.XCad;

namespace SldWorksLookup.View
{
    public class GetObjectByPIDWindowViewModel : ViewModelBase
    {
        private RelayCommand _getObjectCommand;
        private IModelDoc2 _doc;
        private readonly IXApplication _application;
        private string _pID;

        public GetObjectByPIDWindowViewModel(IModelDoc2 doc,IXApplication application)
        {
            _doc = doc;
            _application = application;
        }

        public RelayCommand GetObjectCommand { get => _getObjectCommand ?? (_getObjectCommand = new RelayCommand(GetObjectClick, CanGetObject)); set => Set(ref _getObjectCommand, value); }

        public string PID
        {
            get => _pID; set
            {
                Set(ref _pID ,value);
                GetObjectCommand.RaiseCanExecuteChanged();
            }
        }


        private void GetObjectClick()
        {
            var byteId = Convert.FromBase64String(PID);
            var obj = _doc.Extension.GetObjectByPersistReference3(byteId, out int errorCode);
            var errorCodeEnum = (swPersistReferencedObjectStates_e)errorCode;
            if (obj == null)
            {
                _application.ShowMessageBox($"Cannot get object by PID, {errorCodeEnum.ToString()}");
            }
            else
            {
                var matchtype = PIDComObjectMatcherUtil.Match(obj);
                if (matchtype != null)
                {
                    var insPro = InstanceProperty.Create(obj, matchtype);
                    var window = new LookupPropertyWindow(insPro);
                    window.Show();
                }
                else
                {
                    _application.ShowMessageBox($"{obj.GetType().FullName} cannot get a type, {errorCodeEnum.ToString()}");
                }
            }
        }

        private bool CanGetObject()
        {
            return !string.IsNullOrEmpty(PID);
        }

    }
}
