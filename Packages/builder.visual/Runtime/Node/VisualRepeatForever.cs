// automatically generate from `VisualNodeTemplateCode.cs`
using EntitiesBT.Core;
using EntitiesBT.Nodes;
using EntitiesBT.Components;
using Unity.Entities;
using System;
using Runtime;

namespace EntitiesBT.Builder.Visual
{
    [NodeSearcherItem("EntitiesBT/Node/RepeatForeverNode")]
    [Serializable]
    public class VisualRepeatForever : IVisualBuilderNode
    {
        [PortDescription("")] public InputTriggerPort Parent;
        [PortDescription("")] public OutputTriggerPort Children;

        public EntitiesBT.Core.NodeState BreakStates;

        public INodeDataBuilder GetBuilder(GraphInstance instance, GraphDefinition definition)
        {
            var @this = this;
            return new VisualBuilder<RepeatForeverNode>(BuildImpl, Children.ToBuilderNode(instance, definition));
            void BuildImpl(BlobBuilder blobBuilder, ref EntitiesBT.Nodes.RepeatForeverNode data, INodeDataBuilder self, ITreeNode<INodeDataBuilder>[] builders)
            {
                data.BreakStates = BreakStates;
            }
        }
    }
}
