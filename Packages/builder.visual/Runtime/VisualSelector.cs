using System;
using EntitiesBT.Core;
using EntitiesBT.Nodes;
using Runtime;

namespace EntitiesBT.Builder.Visual
{
    [NodeSearcherItem("EntitiesBT/Build/Selector")]
    [Serializable]
    public struct VisualSelector : INode, IVisualBuilderNode
    {
        [PortDescription("")]
        public InputTriggerPort Parent;

        [PortDescription("")]
        public OutputTriggerMultiPort Children;

        public INodeDataBuilder GetBuilder(GraphDefinition definition)
        {
            return new VisualBuilder<SelectorNode>(Children.ToBuilderNode(definition));
        }
    }
}