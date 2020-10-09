using GalaSoft.MvvmLight;
using System;
using System.Linq;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight.Command;

namespace SldWorksLookup.Model
{

    public class InstanceTree : ObservableObject
    {
        private string _name;
        private RelayCommand _runCommand;
        private NodeStatus _nodeStatus;
        private string _nodeToolTip;

        protected InstanceTree(InstanceProperty instanceProperty)
        {
            InstanceProperty = instanceProperty;
        }

        public static InstanceTree Create(InstanceProperty instanceProperty, string name = null)
        {
            if (instanceProperty == null)
            {
                throw new ArgumentNullException(nameof(instanceProperty));
            }

            InstanceTree tree;
            switch (instanceProperty.InstanceType.FullName)
            {
                case "SolidWorks.Interop.sldworks.ISldWorks":
                    tree = new ISldWorksInstaceTree(instanceProperty);
                    break;
                case "SolidWorks.Interop.sldworks.IModelDoc2":
                    tree = new IModelDoc2InstanceTree(instanceProperty);
                    break;
                case "SolidWorks.Interop.sldworks.IFeature":
                    tree = new IFeatureInstanceTree(instanceProperty);
                    break;
                case "SolidWorks.Interop.sldworks.IBody2":
                    tree = new IBody2InstanceTree(instanceProperty);
                    break;
                case "SolidWorks.Interop.sldworks.IFace2":
                    tree = new IFace2InstanceTree(instanceProperty);
                    break;
                case "SolidWorks.Interop.sldworks.IEdge":
                    tree = new IEdgeInstanceTree(instanceProperty);
                    break;
                case "SolidWorks.Interop.sldworks.ILoop2":
                    tree = new ILoop2InstanceTree(instanceProperty);
                    break;
                case "SolidWorks.Interop.sldworks.IPartDoc":
                    tree = new IPartDocInstanceTree(instanceProperty);
                    break;
                case "SolidWorks.Interop.sldworks.IAssemblyDoc":
                    tree = new IAssemblyDocInstanceTree(instanceProperty);
                    break;
                case "SolidWorks.Interop.sldworks.IDrawingDoc":
                    tree = new IDrawingDocInstanceTree(instanceProperty);
                    break;
                case "SolidWorks.Interop.sldworks.IComponent2":
                    tree = new IComponent2InstanceTree(instanceProperty);
                    break;
                default:
                    tree = new InstanceTree(instanceProperty);
                    break;
            }

            tree.Name = string.IsNullOrEmpty(name) ? instanceProperty?.ToString() : name;

            return tree;
        }

        public ObservableCollection<InstanceTree> Children { get; set; } = new ObservableCollection<InstanceTree>();

        public InstanceProperty InstanceProperty { get; private set; }

        public string Name { get => _name; set => Set(ref _name, value); }

        public NodeStatus NodeStatus
        {
            get => _nodeStatus; set
            {
                Set(ref _nodeStatus, value);
                RunCommand.RaiseCanExecuteChanged();
            }
        }

        public RelayCommand RunCommand { get => _runCommand ?? (_runCommand = new RelayCommand(RunClick, CanRunClick)); set => _runCommand = value; }

        public string NodeToolTip { get => _nodeToolTip; set => Set(ref _nodeToolTip, value); }

        private void RunClick()
        {
            AddNodesLazy();
        }

        private bool CanRunClick()
        {
            return NodeStatus == NodeStatus.NeedRun;
        }

        /// <summary>
        /// 求解未初始化的实例属性
        /// </summary>
        internal void ResolveProperties()
        {
            InstanceProperty?.LazyInit();
        }

        public virtual void AddNodesLazy()
        {
            NodeStatus = NodeStatus.Ok;
        }

        public void AddNodes<TParent, TNode>(Func<TParent, object[]> func, Func<TNode, string> nodeNameFunc = null)
    where TParent : class where TNode : class
        {
            var array = func?.Invoke(InstanceProperty.Instance as TParent);

            //check
            if (array == null)
            {
                return;
            }

            //ToNodes
            var nodes = array.OfType<TNode>()
                .Select(node => InstanceTree.Create(new InstanceProperty(node, typeof(TNode), false), nodeNameFunc?.Invoke(node)));

            //Add to Children
            foreach (var node in nodes)
            {
                Children.Add(node);
            }
        }

        public void AddNode<TParent, TNode>(Func<TParent, object> func, Func<TNode, string> nodeNameFunc = null)
            where TParent : class where TNode : class
        {
            var node = func?.Invoke(InstanceProperty.Instance as TParent) as TNode;

            //check
            if (node == null)
            {
                return;
            }

            var ins = new InstanceProperty(node, typeof(TNode), false);
            if (ins != null)
            {
                Children.Add(InstanceTree.Create(ins, nodeNameFunc?.Invoke(node)));
            }
        }
    }
}
