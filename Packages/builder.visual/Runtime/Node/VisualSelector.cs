using System;
using EntitiesBT.Core;
using EntitiesBT.Nodes;
using Runtime;

namespace EntitiesBT.Builder.Visual
{
    [NodeSearcherItem("EntitiesBT/Node/Selector")]
    [Serializable]
    public struct VisualSelector : IVisualBuilderNode
    {
        [PortDescription("")]
        public InputTriggerPort Parent;

        [PortDescription("")]
        public OutputTriggerPort Children;

        public INodeDataBuilder GetBuilder(GraphInstance instance, GraphDefinition definition)
        {
            return new VisualBuilder<SelectorNode>(Children.ToBuilderNode(instance, definition));
        }
    }
}