using System;
using EntitiesBT.Core;
using EntitiesBT.Nodes;
using EntitiesBT.Variable;
using Runtime;
using Unity.Entities;

namespace EntitiesBT.Builder.Visual
{
    [NodeSearcherItem("EntitiesBT/Build/Timer")]
    [Serializable]
    public struct VisualTimer : INode, IVisualBuilderNode
    {
        [PortDescription("")]
        public InputTriggerPort Parent;

        [PortDescription("")]
        public OutputTriggerPort Child;

        [PortDescription(Runtime.ValueType.Float)]
        public InputDataPort CountdownSeconds;

        public NodeState BreakReturnState;

        public INodeDataBuilder GetBuilder(GraphDefinition definition)
        {
            var @this = this;
            return new VisualBuilder<TimerNode>(BuildImpl, Child.ToBuilderNode(definition).Yield());

            void BuildImpl(BlobBuilder blobBuilder, ref TimerNode data, INodeDataBuilder self, ITreeNode<INodeDataBuilder>[] builders)
            {
                data.BreakReturnState = @this.BreakReturnState;
                @this.CountdownSeconds.ToVariableProperty<float>(definition).Allocate(ref blobBuilder, ref data.CountdownSeconds, self, builders);
            }
        }
    }
}
