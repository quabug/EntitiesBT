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
    [NodeSearcherItem("EntitiesBT/Node/HasComponentNode")]
    [Serializable]
    public class VisualHasComponent : IVisualBuilderNode
    {
        [PortDescription("")] public InputTriggerPort Parent;
        

        public System.UInt64 StableTypeHash;

        public INodeDataBuilder GetBuilder(GraphInstance instance, GraphDefinition definition)
        {
            var @this = this;
            return new VisualBuilder<EntitiesBT.Nodes.HasComponentNode>(BuildImpl, null);
            unsafe void BuildImpl(BlobBuilder blobBuilder, ref EntitiesBT.Nodes.HasComponentNode data, INodeDataBuilder self, ITreeNode<INodeDataBuilder>[] builders)
            {
                data.StableTypeHash = StableTypeHash;
            }
        }
    }
}
