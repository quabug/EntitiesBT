// automatically generate from `NodeComponentTemplateCode.cs`
using EntitiesBT.Core;
using Unity.Entities;

namespace EntitiesBT.Components
{
    public class BTHasComponent : BTNode<EntitiesBT.Nodes.HasComponentNode>
    {
        public System.UInt64 StableTypeHash;
        protected override void Build(ref EntitiesBT.Nodes.HasComponentNode data, BlobBuilder builder, ITreeNode<INodeDataBuilder>[] tree)
        {
            data.StableTypeHash = StableTypeHash;
        }
    }
}
