// automatically generate from `VisualNodeTemplateCode.cs`
using EntitiesBT.Core;
using EntitiesBT.Nodes;
using EntitiesBT.Components;
using Unity.Entities;
using System;
using Runtime;

namespace EntitiesBT.Builder.Visual
{
    [NodeSearcherItem("EntitiesBT/Node/RandomSelectorNode")]
    [Serializable]
    public class VisualRandomSelector : IVisualBuilderNode
    {
        [PortDescription("")] public InputTriggerPort Parent;
        [PortDescription("")] public OutputTriggerPort Children;

        

        public INodeDataBuilder GetBuilder(GraphInstance instance, GraphDefinition definition)
        {
            var @this = this;
            return new VisualBuilder<EntitiesBT.Nodes.RandomSelectorNode>(BuildImpl, Children.ToBuilderNode(instance, definition));
            void BuildImpl(BlobBuilder blobBuilder, ref EntitiesBT.Nodes.RandomSelectorNode data, INodeDataBuilder self, ITreeNode<INodeDataBuilder>[] builders)
            {
                
            }
        }
    }
}
