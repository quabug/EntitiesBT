using System;
using EntitiesBT.Core;
using EntitiesBT.Nodes;
using Runtime;
using Unity.Entities;

namespace EntitiesBT.Builder.Visual
{
    [NodeSearcherItem("EntitiesBT/Build/Forever")]
    [Serializable]
    public struct VisualForever : INode, IVisualBuilderNode
    {
        public NodeState BreakStates;

        [PortDescription("")]
        public InputTriggerPort Parent;

        [PortDescription("")]
        public OutputTriggerPort Child;

        public INodeDataBuilder GetBuilder(GraphDefinition definition)
        {
            return new VisualBuilder<RepeatForeverNode>(BuildImpl, Child.ToBuilderNode(definition).Yield());
        }

        public void BuildImpl(BlobBuilder blobBuilder, ref RepeatForeverNode data, ITreeNode<INodeDataBuilder>[] builders)
        {
            data.BreakStates = BreakStates;
        }
    }
}
