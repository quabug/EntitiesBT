using System;
using EntitiesBT.Core;
using EntitiesBT.Nodes;
using Runtime;
using Unity.Entities;

namespace EntitiesBT.Builder.Visual
{
    [NodeSearcherItem("EntitiesBT/Node/Timer")]
    [Serializable]
    public struct VisualTimer : IVisualBuilderNode
    {
        [PortDescription("")]
        public InputTriggerPort Parent;

        [PortDescription("")]
        public OutputTriggerPort Child;

        [PortDescription(Runtime.ValueType.Float)]
        public InputDataPort CountdownSeconds;

        [PortDescription(Runtime.ValueType.Float)]
        public OutputDataPort CountdownSecondsOutput;

        [PortDescription("CountdownSeconds")]
        public OutputTriggerPort OnCountdownSeconds;

        public NodeState BreakReturnState;

        public INodeDataBuilder GetBuilder(GraphInstance instance, GraphDefinition definition)
        {
            var @this = this;
            return new VisualBuilder<TimerNode>(BuildImpl, Child.ToBuilderNode(instance, definition));

            void BuildImpl(BlobBuilder blobBuilder, ref TimerNode data, INodeDataBuilder self, ITreeNode<INodeDataBuilder>[] builders)
            {
                data.BreakReturnState = @this.BreakReturnState;
                @this.CountdownSeconds.ToVariablePropertyReader<float>(instance, definition)
                    .Allocate(ref blobBuilder, ref data.CountdownSecondsReader, self, builders)
                ;
            }
        }
    }
}
