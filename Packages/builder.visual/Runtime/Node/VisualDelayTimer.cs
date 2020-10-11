// automatically generate from `VisualNodeTemplateCode.cs`
using EntitiesBT.Core;
using EntitiesBT.Nodes;
using EntitiesBT.Components;
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
        

        [PortDescription(Runtime.ValueType.Float)] public InputDataPort TimerSeconds;

        public INodeDataBuilder GetBuilder(GraphInstance instance, GraphDefinition definition)
        {
            var @this = this;
            return new VisualBuilder<DelayTimerNode>(BuildImpl, null);
            void BuildImpl(BlobBuilder blobBuilder, ref EntitiesBT.Nodes.DelayTimerNode data, INodeDataBuilder self, ITreeNode<INodeDataBuilder>[] builders)
            {
                @this.TimerSeconds.ToVariablePropertyReadWrite<System.Single>(instance, definition).Allocate(ref blobBuilder, ref data.TimerSeconds, self, builders);
            }
        }
    }
}
