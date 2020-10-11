// automatically generate from `VisualNodeTemplateCode.cs`
using EntitiesBT.Core;
using EntitiesBT.Nodes;
using EntitiesBT.Components;
using Unity.Entities;
using System;
using Runtime;

namespace EntitiesBT.Builder.Visual
{
    [NodeSearcherItem("EntitiesBT/Node/RunOnMainThreadNode")]
    [Serializable]
    public class VisualRunOnMainThread : IVisualBuilderNode
    {
        [PortDescription("")] public InputTriggerPort Parent;
        [PortDescription("")] public OutputTriggerPort Children;

        

        public INodeDataBuilder GetBuilder(GraphInstance instance, GraphDefinition definition)
        {
            var @this = this;
            return new VisualBuilder<RunOnMainThreadNode>(BuildImpl, Children.ToBuilderNode(instance, definition));
            void BuildImpl(BlobBuilder blobBuilder, ref EntitiesBT.Nodes.RunOnMainThreadNode data, INodeDataBuilder self, ITreeNode<INodeDataBuilder>[] builders)
            {
                
            }
        }
    }
}
