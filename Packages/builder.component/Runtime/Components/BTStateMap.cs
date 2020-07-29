// automatically generate from `NodeComponentTemplateCode.cs`
using EntitiesBT.Core;
using Unity.Entities;

namespace EntitiesBT.Components
{
    public class BTStateMap : BTNode<EntitiesBT.Nodes.StateMapNode>
    {
        public EntitiesBT.Core.NodeState MapSuccess;
        public EntitiesBT.Core.NodeState MapFailure;
        public EntitiesBT.Core.NodeState MapRunning;
        public EntitiesBT.Core.NodeState MapError;
        protected override void Build(ref EntitiesBT.Nodes.StateMapNode data, BlobBuilder builder, ITreeNode<INodeDataBuilder>[] tree)
        {
            data.MapSuccess = MapSuccess;
            data.MapFailure = MapFailure;
            data.MapRunning = MapRunning;
            data.MapError = MapError;
        }
    }
}
