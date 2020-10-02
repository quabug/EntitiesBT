using System;
using EntitiesBT.Core;
using EntitiesBT.Nodes;
using Runtime;
using Unity.Entities;

namespace EntitiesBT.Builder.Visual
{
    [NodeSearcherItem("EntitiesBT/Build/Forever")]
    [Serializable]
    public struct VisualForever : IVisualBuilderNode
    {
        public NodeState BreakStates;

        [PortDescription("")]
        public InputTriggerPort Parent;

        [PortDescription("")]
        public OutputTriggerPort Child;

        public INodeDataBuilder GetBuilder(GraphInstance instance, GraphDefinition definition)
        {
            return new VisualBuilder<RepeatForeverNode>(BuildImpl, Child.ToBuilderNode(instance, definition));
        }

        public void BuildImpl(BlobBuilder blobBuilder, ref RepeatForeverNode data, INodeDataBuilder self, ITreeNode<INodeDataBuilder>[] builders)
        {
            data.BreakStates = BreakStates;
        }

        public Execution Execute<TCtx>(TCtx ctx, InputTriggerPort port) where TCtx : IGraphInstance => Execution.Done;
    }
}
