// automatically generate from `VisualNodeTemplateCode.cs`
using EntitiesBT.Core;
using EntitiesBT.Nodes;
using EntitiesBT.Components;
using Unity.Entities;
using System;
using Runtime;

namespace EntitiesBT.Builder.Visual
{
    [NodeSearcherItem("EntitiesBT/Node/SuccessNode")]
    [Serializable]
    public class VisualSuccess : IVisualBuilderNode
    {
        [PortDescription("")] public InputTriggerPort Parent;
        

        

        public INodeDataBuilder GetBuilder(GraphInstance instance, GraphDefinition definition)
        {
            var @this = this;
            return new VisualBuilder<SuccessNode>(BuildImpl, null);
            void BuildImpl(BlobBuilder blobBuilder, ref EntitiesBT.Nodes.SuccessNode data, INodeDataBuilder self, ITreeNode<INodeDataBuilder>[] builders)
            {
                
            }
        }
    }
}
