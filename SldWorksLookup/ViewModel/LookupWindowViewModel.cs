using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using SldWorksLookup.Model;
using SolidWorks.Interop.sldworks;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Xarial.XCad.SolidWorks;

namespace SldWorksLookup.ViewModel
{
    public class LookupWindowViewModel : ViewModelBase
    {
        #region Fields

        private RelayCommand _exitCommand;
        private ObservableCollection<InstanceTree> _trees = new ObservableCollection<InstanceTree>();
        private InstanceTree _selectedTreeItem;
        private LookupProperties _properties;
        private RelayCommand _runCommand;
        private Visibility _runButtonVisibility;
        private object selectedProperty;
        private RelayCommand _helpNavigateCommand;

        #endregion

        /// <summary>
        /// 负责通知界面关闭或者刷新
        /// </summary>
        public event Action<int> ExitOrUpdate;

        #region ctor

        public LookupWindowViewModel(SwApplication extension)
        {
            SwApplication = extension;
            var insProperty = InstanceProperty.Create(SwApplication.Sw, typeof(ISldWorks));
            var treeItem = InstanceTree.Create(insProperty);
            Trees.Add(treeItem);
            SelectedTreeItem = treeItem;
        }

        public LookupWindowViewModel(InstanceProperty instanceProperty)
        {
            var treeItem = InstanceTree.Create(instanceProperty);
            Trees.Add(treeItem);
            SelectedTreeItem = treeItem;
        }

        public LookupWindowViewModel(IList<InstanceProperty> insProperties)
        {
            var treeItems = insProperties.Select(p => InstanceTree.Create(p));
            Trees = new ObservableCollection<InstanceTree>(treeItems);
        }

        #endregion

        #region Properties

        public SwApplication SwApplication { get; set; }

        public LookupProperties Properties { get => _properties; set => Set(ref _properties, value); }

        public RelayCommand ExitCommand { get => _exitCommand ?? (_exitCommand = new RelayCommand(ExitClick, () => true)); set => _exitCommand = value; }

        public RelayCommand RunCommand { get => _runCommand ?? (_runCommand = new RelayCommand(RunClick, CanRunClick)); set => _runCommand = value; }

        public RelayCommand HelpNavigateCommand { get => _helpNavigateCommand ?? (_helpNavigateCommand = new RelayCommand(HelpNavigate,CanHelpNavigate)); set => _helpNavigateCommand = value; }

        public ObservableCollection<InstanceTree> Trees { get => _trees; set => Set(ref _trees, value); }

        public InstanceTree SelectedTreeItem
        {
            get => _selectedTreeItem; set
            {
                Set(ref _selectedTreeItem, value);
                Properties = SelectedTreeItem?.InstanceProperty?.Properties;
                RunCommand.RaiseCanExecuteChanged();
            }
        }

        public Visibility RunButtonVisibility { get => _runButtonVisibility; set => Set(ref _runButtonVisibility, value); }

        public object SelectedProperty
        {
            get => selectedProperty; set
            {
                Set(ref selectedProperty, value);
                HelpNavigateCommand.RaiseCanExecuteChanged();
            }
        }

        #endregion

        #region Methods

        private void ExitClick()
        {
            ExitOrUpdate?.Invoke(0);
        }

        private void RunClick()
        {
            var properties = SelectedTreeItem?.InstanceProperty;
            if (properties != null && properties is MethodInstanceProperty methodInstance)
            {
                try
                {
                    methodInstance.Invoke();
                    //更新属性
                    ExitOrUpdate?.Invoke(1);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private bool CanRunClick()
        {
            bool flag = false;

            var properties = SelectedTreeItem?.InstanceProperty;
            if (properties == null)
            {
                flag = false;
            }
            else
            {
                if (properties is MethodInstanceProperty)
                {
                    flag = true;
                }
            }

            RunButtonVisibility = flag ? Visibility.Visible : Visibility.Collapsed;
            return flag;
        }

        private bool CanHelpNavigate()
        {
            return true;
        }

        private void HelpNavigate()
        {
            //TODO 导航到帮助页面
            if (SelectedProperty == null)
            {
                return;
            }

        }

        #endregion
    }
}
