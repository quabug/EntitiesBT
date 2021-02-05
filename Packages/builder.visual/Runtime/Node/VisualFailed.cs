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
    [NodeSearcherItem("EntitiesBT/Node/FailedNode")]
    [Serializable]
    public class VisualFailed : IVisualBuilderNode
    {
        [PortDescription("")] public InputTriggerPort Parent;
        

        

        public INodeDataBuilder GetBuilder(GraphInstance instance, GraphDefinition definition)
        {
            var @this = this;
            return new VisualBuilder<EntitiesBT.Nodes.FailedNode>(BuildImpl, null);
            unsafe void BuildImpl(BlobBuilder blobBuilder, ref EntitiesBT.Nodes.FailedNode data, INodeDataBuilder self, ITreeNode<INodeDataBuilder>[] builders)
            {
                
            }
        }
    }
}
