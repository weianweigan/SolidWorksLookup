using GalaSoft.MvvmLight;
using SldWorksLookup.Model;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Xarial.XCad;

namespace SldWorksLookup.View
{
    public class SelectionPIDViewModel : ViewModelBase
    {
        private readonly IXApplication _application;
        private ObservableCollection<SelectionPID> _selectionPIDs;
        private SelectionPID _selectedPIDItem;

        public SelectionPIDViewModel(ICollection<SelectionPID> pids,IXApplication application)
        {
            SelectionPIDs = new ObservableCollection<SelectionPID>(pids);
            _application = application;
        }

        public ObservableCollection<SelectionPID> SelectionPIDs { get => _selectionPIDs; set => Set(ref _selectionPIDs, value); }

        public SelectionPID SelectedPIDItem { get => _selectedPIDItem; set =>Set(ref _selectedPIDItem ,value); }

        public void DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (SelectedPIDItem == null)
            {
                return;
            }

            try
            {

                var matchType = SelectTypeMatcherUtil.Match(SelectedPIDItem.SelectType);
                var insPro = InstanceProperty.Create(SelectedPIDItem.SelectedObject, matchType);
                var window = new LookupPropertyWindow(insPro);
                window.Show();

            }
            catch (System.Exception ex)
            {
                _application.ShowMessageBox($"Error:{ex.Message}");
            }
        }

    }
}
