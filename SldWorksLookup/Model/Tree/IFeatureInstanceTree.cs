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
            NodeToolTip = $"{nameof(IFeature.GetDefinition)} && {nameof(IFeature.GetSpecificFeature2)}";
        }

        private void AddSubFeatureNodes()
        {
            AddNodes<IFeature, IFeature>(feat => feat.GetSubFeats().ToArray(),
                node => $"{node.Name}({nameof(IFeature)})");
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

                    types = TypeNameToDefinitionUtil.Match(typeName);

                    foreach (var type in types)
                    {
                        AddNode<IFeature>(f => f.GetDefinition(), 
                            type, _ => $"{type.Name}({typeName})");
                        AddNode<IFeature>(f => f.GetSpecificFeature2(),
                            type, _ => $"{type.Name}({typeName})");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        [Obsolete]
        private void AddBodyNode()
        {
            AddNode<IFeature, IBody2>(feat => feat.IGetBody2(),
                node => $"{node.Name}({nameof(IBody2)})");
        }

    }
}
