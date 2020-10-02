using System;
using EntitiesBT.Core;
using EntitiesBT.Nodes;
using Runtime;

namespace EntitiesBT.Builder.Visual
{
    [NodeSearcherItem("EntitiesBT/Build/Sequence")]
    [Serializable]
    public struct VisualSequence : IVisualBuilderNode
    {
        [PortDescription("")]
        public InputTriggerPort Parent;

        [PortDescription("")]
        public OutputTriggerPort Children;

        public INodeDataBuilder GetBuilder(GraphInstance instance, GraphDefinition definition)
        {
            return new VisualBuilder<SelectorNode>(Children.ToBuilderNode(instance, definition));
        }

        public Execution Execute<TCtx>(TCtx ctx, InputTriggerPort port) where TCtx : IGraphInstance => Execution.Done;
    }
}
