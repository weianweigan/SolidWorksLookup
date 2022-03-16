using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Xarial.XCad.Base.Attributes;
using Xarial.XCad.SolidWorks;
using Xarial.XCad.UI.Commands;
using SldWorksLookup.Properties;
using SldWorksLookup.Model;
using SolidWorks.Interop.swconst;
using System.Diagnostics;
using SolidWorks.Interop.sldworks;
using Xarial.XCad.Base.Enums;
using SldWorksLookup.View;
using System;
using System.Linq;

namespace SldWorksLookup
{
    [ComVisible(true)]
    [Title("SolidWorks Lookup")]
    [Description("Lookup SolidWorks Objects 0.0.3")]
    [Icon(typeof(Resource),nameof(Resource.BrowseData_16x))]
    public class AddIn:SwAddInEx
    {
        public override void OnConnect()
        {
            var cmdGroup = CommandManager.AddCommandGroup<Command_e>();
            cmdGroup.CommandClick += CmdGroup_CommandClick;
            //cmdGroup.CommandStateResolve += CmdGroup_CommandStateResolve;
        }

        private void CmdGroup_CommandStateResolve(Command_e spec, Xarial.XCad.UI.Commands.Structures.CommandState state)
        {
            switch (spec)
            {
                case Command_e.Lookup:
                    state.Enabled = true;
                    break;
                case Command_e.CurrentSelection:
                    var seleCount = Application?.Documents?.Active?.Selections.Count;
                    state.Enabled = seleCount.HasValue ? seleCount.Value > 0 : false;
                    break;               
                default:
                    break;
            }
        }

        private void CmdGroup_CommandClick(Command_e spec)
        {
            switch (spec)
            {
                case Command_e.Lookup:

                    SnoopISldWorks();
                    break;

                case Command_e.ActiveDoc:

                    SnoopActiveDoc();
                    break;

                case Command_e.CurrentSelection:

                    SnoopCurrentSelection();
                    break;

                case Command_e.SnoopPID:

                    SnoopPID();
                    break;

                case Command_e.SnoopAdvancedHole:
                    SnoopAdvancedHole();
                    break;

                case Command_e.GetObjectByPID:
                    GetObject();
                    break;

                case Command_e.CaptureCmd:
                    ShowCaptureWindow();
                    break;

                case Command_e.ColorToInt:
                    ShowColorWindow();
                    break;

                //case Command_e.SketchPathSplit:
                //    {
                //        var doc = Application.Sw.IActiveDoc2;
                //        if (doc != null)
                //        {
                //            //批量导出
                //            {
                //                var pathSolver = new PathSplit.ModelPathsSolver(doc);
                //                pathSolver.Export();
                //            }

                //            //使用UI导出
                //            //{
                //            //    var window = new PathSplit.PathSplitWindow(doc, Application.WindowHandle);
                //            //    window.Show();
                //            //}
                //        }
                //        else
                //            Application.ShowMessageBox("未打开文档");

                //    } 
                //    break;

                //case Command_e.AddinManager:

                    //Application.ShowMessageBox("开发中");

                    //break;

                case Command_e.TestFramework:

                    Process.Start(new ProcessStartInfo("https://github.com/weianweigan/SldWorks.TestRunner"));
                    break;
            }
        }

        private void SnoopAdvancedHole()
        {
            var doc = this.Application.Sw.IActiveDoc2;
            if (doc == null)
            {
                Application.Sw.SendMsgToUser("No ActiveDoc");
                return;
            }
            var count = doc.ISelectionManager.GetSelectedObjectCount();
            var ins = new List<InstanceProperty>();

            var mark = doc.ISelectionManager.GetSelectedObjectMark(1);
            var obj = doc.ISelectionManager.GetSelectedObject6(1, mark);
            var type = (swSelectType_e)doc.ISelectionManager.GetSelectedObjectType3(1, mark);

            if (type != swSelectType_e.swSelBODYFEATURES)
            {
                Application.ShowMessageBox("Please a advancedhole feature");
                return;
            }

            var feat = obj as IFeature;

            if (feat == null || feat.GetTypeName2() != "AdvHoleWzd")
            {
                Application.ShowMessageBox("Please a advancedhole feature");
                return;
            }

            try
            {
                var featData = feat.GetDefinition() as IAdvancedHoleFeatureData;

                featData.AccessSelections(doc, null);

                var elems =(featData.GetNearSideElements() as object[]).Cast<IAdvancedHoleElementData>().ToList();

                var types = new List<Type>() {
                    typeof(ICounterboreElementData),
                    typeof(ICountersinkElementData),
                    typeof(IStraightElementData),
                    typeof(IStraightTapElementData ),
                    typeof(ITaperedTapElementData)};

                foreach (var ele in elems)
                {
                    var matchtype = types.FirstOrDefault(p => p.IsInstanceOfType(ele));

                    ins.Add(InstanceProperty.Create(ele,matchtype ?? typeof(IAdvancedHoleElementData)));
                }
                ins.Add(InstanceProperty.Create("Base Type", typeof(string)));
                foreach (var ele in elems)
                {
                    ins.Add(InstanceProperty.Create(ele,typeof(IAdvancedHoleElementData)));
                }

                featData.ReleaseSelectionAccess();
            }
            catch (System.Exception ex)
            {
                Application.Sw.SendMsgToUser($"{ex.Message},{type} Cannot match a SolidWorks Interface");
            }

            var selPpopWindow = CreatePopupWindow<View.LookupPropertyWindow>();
            selPpopWindow.Control.MutiInit(ins);
            selPpopWindow.Show();
        }

        private void ShowColorWindow()
        {
            var window = CreatePopupWindow<ColorToIntWindow>();
            window?.Show();
        }

        private void ShowCaptureWindow()
        {
            var window = new CaptureCmd(Application);
            window?.Show();
        }

        private void GetObject()
        {
            var doc = Application.Sw.IActiveDoc2;
            if (doc == null)
            {
                Application.ShowMessageBox($"No active doc");
            }
            var getObjectVM = new GetObjectByPIDWindowViewModel(doc, this.Application);
            var window = CreatePopupWindow<GetObjectByPIDWindow>();
            window.Control.VM = getObjectVM;
            window.Show();
        }

        private void SnoopISldWorks()
        {
            var popWindow = CreatePopupWindow<View.LookupPropertyWindow>();
            popWindow.Control.ISldWorksInit(this.Application);
            popWindow.Show();
        }

        private void SnoopActiveDoc()
        {
            var mdlDoc = Application.Sw.IActiveDoc2;
            if (mdlDoc == null)
            {
                Application.Sw.SendMsgToUser("No ActiveDoc");
                return;
            }
            var docWindow = CreatePopupWindow<View.LookupPropertyWindow>();
            docWindow.Control.MutiInit(new InstanceProperty[] {  InstanceProperty.Create(mdlDoc, typeof(IModelDoc2)) });
            docWindow.Show();
        }

        private void SnoopCurrentSelection()
        {
            var doc = this.Application.Sw.IActiveDoc2;
            if (doc == null)
            {
                Application.Sw.SendMsgToUser("No ActiveDoc");
                return;
            }
            var count = doc.ISelectionManager.GetSelectedObjectCount();
            var ins = new List<InstanceProperty>();
            for (int i = 1; i < count + 1; i++)
            {
                var mark = doc.ISelectionManager.GetSelectedObjectMark(i);
                var obj = doc.ISelectionManager.GetSelectedObject6(i, mark);
                var type = (swSelectType_e)doc.ISelectionManager.GetSelectedObjectType3(i, mark);

                try
                {
                    var matchType = SelectTypeMatcherUtil.Match(type);
                    if (matchType != null)
                    {
                        var insPro = InstanceProperty.Create(obj, matchType);
                        ins.Add(insPro);
                    }
                    else
                    {
                        Application.Sw.SendMsgToUser($"{type} Cannot match a SolidWorks Interface");
                    }
                }
                catch (System.Exception ex)
                {
                    Application.Sw.SendMsgToUser($"{ex.Message},{type} Cannot match a SolidWorks Interface");
                }

            }
            var selPpopWindow = CreatePopupWindow<View.LookupPropertyWindow>();
            selPpopWindow.Control.MutiInit(ins);
            selPpopWindow.Show();
        }

        private void SnoopPID()
        {
            var doc = Application.Documents.Active?.Model;
            if (doc == null)
            {
                Application.ShowMessageBox("No active doc", MessageBoxIcon_e.Error);
                return;
            }

            var count = doc.ISelectionManager.GetSelectedObjectCount();

            List<SelectionPID> pids = new List<SelectionPID>();

            for (int i = 1; i < count + 1; i++)
            {
                var mark = doc.ISelectionManager.GetSelectedObjectMark(i);
                var obj = doc.ISelectionManager.GetSelectedObject6(i, mark);
                var type = (swSelectType_e)doc.ISelectionManager.GetSelectedObjectType3(i, mark);
                var pid = doc.Extension.GetPersistReference3(obj);

                pids.Add(SelectionPID.Create(type, obj, mark,pid));
            }

            var window = CreatePopupWindow<SelectionPIDWindow>();
            window.Control.VM = new SelectionPIDViewModel(pids,Application);
            window?.Show();
        }
    }
}
