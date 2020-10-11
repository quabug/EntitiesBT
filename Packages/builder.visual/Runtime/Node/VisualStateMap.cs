// automatically generate from `VisualNodeTemplateCode.cs`
using EntitiesBT.Core;
using EntitiesBT.Nodes;
using EntitiesBT.Components;
using Unity.Entities;
using System;
using Runtime;

namespace EntitiesBT.Builder.Visual
{
    [NodeSearcherItem("EntitiesBT/Node/StateMapNode")]
    [Serializable]
    public class VisualStateMap : IVisualBuilderNode
    {
        [PortDescription("")] public InputTriggerPort Parent;
        [PortDescription("")] public OutputTriggerPort Children;

        public EntitiesBT.Core.NodeState MapSuccess;
        public EntitiesBT.Core.NodeState MapFailure;
        public EntitiesBT.Core.NodeState MapRunning;
        public EntitiesBT.Core.NodeState MapError;

        public INodeDataBuilder GetBuilder(GraphInstance instance, GraphDefinition definition)
        {
            var @this = this;
            return new VisualBuilder<StateMapNode>(BuildImpl, Children.ToBuilderNode(instance, definition));
            void BuildImpl(BlobBuilder blobBuilder, ref EntitiesBT.Nodes.StateMapNode data, INodeDataBuilder self, ITreeNode<INodeDataBuilder>[] builders)
            {
                data.MapSuccess = MapSuccess;
                data.MapFailure = MapFailure;
                data.MapRunning = MapRunning;
                data.MapError = MapError;
            }
        }
    }
}
