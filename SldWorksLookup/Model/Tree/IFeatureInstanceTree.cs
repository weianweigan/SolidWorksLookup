using SolidWorks.Interop.sldworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace SldWorksLookup.Model
{
    public class IFeatureInstanceTree : InstanceTree
    {
        public IFeatureInstanceTree(InstanceProperty instanceProperty) : base(instanceProperty)
        {
            AddSubFeatureNodes();

            NodeStatus = NodeStatus.NeedRun;
            NodeToolTip = $"{nameof(IFeature.GetDefinition)}";
        }

        [Obsolete]
        private void AddBodyNode()
        {
            AddNode<IFeature, IBody2>(feat => feat.IGetBody2(), node => $"{node.Name}({nameof(IBody2)})");
        }

        private void AddSubFeatureNodes()
        {
            AddNodes<IFeature, IFeature>(feat => feat.GetSubFeats().ToArray(),node => $"{node.Name}({nameof(IFeature)})");
        }

        /// <summary>
        /// 获取FeatData
        /// </summary>
        public override void AddNodesLazy()
        {
            var types = default(List<Type>);

            try
            {
                if (InstanceProperty?.Instance is IFeature feat)
                {
                    var typeName = feat.GetTypeName2();

                    types = TypeNameToDefinition.Match(typeName);

                    foreach (var type in types)
                    {
                        AddNode<IFeature>(f => f.GetDefinition(), type, obj => type.Name);
                        AddNode<IFeature>(f => f.GetSpecificFeature2(), type, obj => type.Name);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
