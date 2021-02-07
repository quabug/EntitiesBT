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
    [NodeSearcherItem("EntitiesBT/Node/DelayTimerNode")]
    [Serializable]
    public class VisualDelayTimer : IVisualBuilderNode
    {
        [PortDescription("")] public InputTriggerPort Parent;
        

        [PortDescription(Runtime.ValueType.Float, "TimerSeconds")] public InputDataPort InputTimerSeconds;
        [PortDescription(Runtime.ValueType.Float, "TimerSeconds")] public OutputDataPort OutputTimerSeconds;
        public bool IsLinkedTimerSeconds;

        public INodeDataBuilder GetBuilder(GraphInstance instance, GraphDefinition definition)
        {
            var @this = this;
            return new VisualBuilder<EntitiesBT.Nodes.DelayTimerNode>(BuildImpl, null);
            unsafe void BuildImpl(BlobBuilder blobBuilder, ref EntitiesBT.Nodes.DelayTimerNode data, INodeDataBuilder self, ITreeNode<INodeDataBuilder>[] builders)
            {
                new DataPortReaderAndWriter(@this.IsLinkedTimerSeconds, @this.InputTimerSeconds, @this.OutputTimerSeconds).ToVariantReaderAndWriter<System.Single>(instance, definition).Allocate(ref blobBuilder, ref data.TimerSeconds, self, builders);
            }
        }
    }
}
