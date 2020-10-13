// automatically generate from `VisualNodeTemplateCode.cs`
using EntitiesBT.Core;
using EntitiesBT.Nodes;
using EntitiesBT.Components;
using Unity.Entities;
using System;
using Runtime;

namespace EntitiesBT.Builder.Visual
{
    [NodeSearcherItem("EntitiesBT/Node/ModifyPriorityNode")]
    [Serializable]
    public class VisualModifyPriority : IVisualBuilderNode
    {
        [PortDescription("")] public InputTriggerPort Parent;
        

        public System.Int32 PrioritySelectorIndex;
        public System.Int32 WeightIndex;
        public System.Int32 AddWeight;

        public INodeDataBuilder GetBuilder(GraphInstance instance, GraphDefinition definition)
        {
            var @this = this;
            return new VisualBuilder<EntitiesBT.Nodes.ModifyPriorityNode>(BuildImpl, null);
            void BuildImpl(BlobBuilder blobBuilder, ref EntitiesBT.Nodes.ModifyPriorityNode data, INodeDataBuilder self, ITreeNode<INodeDataBuilder>[] builders)
            {
                data.PrioritySelectorIndex = PrioritySelectorIndex;
                data.WeightIndex = WeightIndex;
                data.AddWeight = AddWeight;
            }
        }
    }
}
