// automatically generate from `VisualNodeTemplateCode.cs`
using EntitiesBT.Core;
using EntitiesBT.Nodes;
using EntitiesBT.Components;
using Unity.Entities;
using System;
using Runtime;

namespace EntitiesBT.Builder.Visual
{
    [NodeSearcherItem("EntitiesBT/Node/RepeatTimesNode")]
    [Serializable]
    public class VisualRepeatTimes : IVisualBuilderNode
    {
        [PortDescription("")] public InputTriggerPort Parent;
        [PortDescription("")] public OutputTriggerPort Children;

        public System.Int32 TickTimes;
        public EntitiesBT.Core.NodeState BreakStates;

        public INodeDataBuilder GetBuilder(GraphInstance instance, GraphDefinition definition)
        {
            var @this = this;
            return new VisualBuilder<RepeatTimesNode>(BuildImpl, Children.ToBuilderNode(instance, definition));
            void BuildImpl(BlobBuilder blobBuilder, ref EntitiesBT.Nodes.RepeatTimesNode data, INodeDataBuilder self, ITreeNode<INodeDataBuilder>[] builders)
            {
                data.TickTimes = TickTimes;
                data.BreakStates = BreakStates;
            }
        }
    }
}
