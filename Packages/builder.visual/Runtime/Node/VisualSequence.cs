using System;
using EntitiesBT.Core;
using EntitiesBT.Nodes;
using Runtime;

namespace EntitiesBT.Builder.Visual
{
    [NodeSearcherItem("EntitiesBT/Node/Sequence")]
    [Serializable]
    public struct VisualSequence : IVisualBuilderNode
    {
        [PortDescription("")]
        public InputTriggerPort Parent;

        [PortDescription("")]
        public OutputTriggerPort Children;

        public INodeDataBuilder GetBuilder(GraphInstance instance, GraphDefinition definition)
        {
            return new VisualBuilder<SequenceNode>(Children.ToBuilderNode(instance, definition));
        }
    }
}
