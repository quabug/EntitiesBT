// automatically generate from `VisualNodeTemplateCode.cs`
using EntitiesBT.Core;
using EntitiesBT.Nodes;
using EntitiesBT.Components;
using EntitiesBT.Variant;
using Unity.Entities;
using System;
using Runtime;

namespace EntitiesBT.Builder.Visual
{
    [NodeSearcherItem("EntitiesBT/Node/TimerNode")]
    [Serializable]
    public class VisualTimer : IVisualBuilderNode
    {
        [PortDescription("")] public InputTriggerPort Parent;
        [PortDescription("")] public OutputTriggerPort Children;

        [PortDescription(Runtime.ValueType.Float, "CountdownSeconds")] public InputDataPort InputCountdownSeconds;
        [PortDescription(Runtime.ValueType.Float, "CountdownSeconds")] public OutputDataPort OutputCountdownSeconds;
        public bool IsLinkedCountdownSeconds;
        public EntitiesBT.Core.NodeState BreakReturnState;

        public INodeDataBuilder GetBuilder(GraphInstance instance, GraphDefinition definition)
        {
            var @this = this;
            return new VisualBuilder<EntitiesBT.Nodes.TimerNode>(BuildImpl, Children.ToBuilderNode(instance, definition));
            unsafe void BuildImpl(BlobBuilder blobBuilder, ref EntitiesBT.Nodes.TimerNode data, INodeDataBuilder self, ITreeNode<INodeDataBuilder>[] builders)
            {
                new DataPortReaderAndWriter(@this.IsLinkedCountdownSeconds, @this.InputCountdownSeconds, @this.OutputCountdownSeconds).ToVariantReaderAndWriter<System.Single>(instance, definition).Allocate(ref blobBuilder, ref data.CountdownSeconds, self, builders);
                data.BreakReturnState = BreakReturnState;
            }
        }
    }
}
