// automatically generate from `VisualNodeTemplateCode.cs`
using EntitiesBT.Core;
using EntitiesBT.Nodes;
using EntitiesBT.Components;
using Unity.Entities;
using System;
using Runtime;

namespace EntitiesBT.Builder.Visual
{
    [NodeSearcherItem("EntitiesBT/Node/RepeatDurationNode")]
    [Serializable]
    public class VisualRepeatDuration : IVisualBuilderNode
    {
        [PortDescription("")] public InputTriggerPort Parent;
        [PortDescription("")] public OutputTriggerPort Children;

        public System.Single CountdownSeconds;
        public EntitiesBT.Core.NodeState BreakStates;

        public INodeDataBuilder GetBuilder(GraphInstance instance, GraphDefinition definition)
        {
            var @this = this;
            return new VisualBuilder<EntitiesBT.Nodes.RepeatDurationNode>(BuildImpl, Children.ToBuilderNode(instance, definition));
            void BuildImpl(BlobBuilder blobBuilder, ref EntitiesBT.Nodes.RepeatDurationNode data, INodeDataBuilder self, ITreeNode<INodeDataBuilder>[] builders)
            {
                data.CountdownSeconds = CountdownSeconds;
                data.BreakStates = BreakStates;
            }
        }
    }
}
