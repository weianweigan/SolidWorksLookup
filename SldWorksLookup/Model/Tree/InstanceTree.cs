using GalaSoft.MvvmLight;
using System;
using System.Linq;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight.Command;
using System.Collections.Generic;
using System.Reflection;
using Exceptionless;

namespace SldWorksLookup.Model
{

    public class InstanceTree : ObservableObject
    {
        #region Fields

        private string _name;
        private RelayCommand _runCommand;
        private NodeStatus _nodeStatus;
        private string _nodeToolTip;

        #endregion

        #region Ctor

        protected InstanceTree(InstanceProperty instanceProperty)
        {
            InstanceProperty = instanceProperty;
        }

        /// <summary>
        /// 根据属性创建不同的树节点的工厂方法
        /// </summary>
        /// <param name="instanceProperty">特点类型的属性</param>
        /// <param name="name">节点名称，默认为接口名称。</param>
        /// <returns>类型为<see cref="InstanceTree"/>的树节点</returns>
        public static InstanceTree Create(
            InstanceProperty instanceProperty, 
            string name = null)
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

            LogExtension.Client?.CreateFeatureUsage(
                $"SnoopType:{instanceProperty?.InstanceType?.FullName}")
                .AddTags("SnoopType");

            return tree;
        }

        #endregion

        #region Properties

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

        #endregion

        #region Methods

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

        /// <summary>
        /// 懒加载节点的虚方法
        /// </summary>
        public virtual void AddNodesLazy()
        {
            NodeStatus = NodeStatus.Ok;
        }

        /// <summary>
        /// 添加多个子节点
        /// </summary>
        /// <typeparam name="TParent">父节点类型</typeparam>
        /// <typeparam name="TNode">子节点类型</typeparam>
        /// <param name="func">获取子节点</param>
        /// <param name="nodeNameFunc">对子节点命名</param>
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
                .Select(node => Create(InstanceProperty.Create(node, typeof(TNode), false), 
                    nodeNameFunc?.Invoke(node)));

            //Add to Children
            foreach (var node in nodes)
            {
                Children.Add(node);
            }
        }

        /// <summary>
        /// 添加单个节点
        /// </summary>
        /// <typeparam name="TParent">父节点类型</typeparam>
        /// <typeparam name="TNode">子节点类型</typeparam>
        /// <param name="func">获取单个子节点</param>
        /// <param name="nodeNameFunc">对</param>
        public void AddNode<TParent, TNode>(Func<TParent, object> func, Func<TNode, string> nodeNameFunc = null)
            where TParent : class where TNode : class
        {
            var parent = InstanceProperty.Instance as TParent;

            if (parent == null)
            {
                return;
            }

            var node = func?.Invoke(parent) as TNode;

            //check
            if (node == null)
            {
                return;
            }

            var ins = InstanceProperty.Create(node, typeof(TNode), false);
            if (ins != null)
            {
                Children.Add(InstanceTree.Create(ins, nodeNameFunc?.Invoke(node)));
            }
        }

        /// <summary>
        /// 添加单个节点--适用于子节点类型不确定的情况
        /// </summary>
        /// <typeparam name="TParent">父节点类型</typeparam>
        /// <param name="func">获取子节点</param>
        /// <param name="nodeType">子节点类型</param>
        /// <param name="nodeNameFunc">对子节点命名</param>
        public void AddNode<TParent>(Func<TParent, object> func,Type nodeType ,Func<object, string> nodeNameFunc = null)
            where TParent : class
        {
            var parent = InstanceProperty.Instance as TParent;

            if (parent == null)
            {
                return;
            }

            var node = func?.Invoke(parent);

            //check
            if (node == null)
            {
                return;
            }
            
            //类型转换和生成属性
            if (nodeType.IsInstanceOfType(node))
            {
                var ins = InstanceProperty.Create(node, nodeType, false);
                if (ins != null)
                {
                    Children.Add(InstanceTree.Create(ins, nodeNameFunc?.Invoke(node)));
                }
            }
        }

        #endregion
    }
}
