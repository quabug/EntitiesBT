using System;
using EntitiesBT.Core;
using Unity.Entities;

namespace EntitiesBT.Components
{
    public class BTHasComponent : BTNode<EntitiesBT.Nodes.HasComponentNode>
    {
        [SerializedType(typeof(IComponentData))] public string TypeName;
        protected override void Build(ref EntitiesBT.Nodes.HasComponentNode data, BlobBuilder builder, ITreeNode<INodeDataBuilder>[] tree)
        {
            var type = Type.GetType(TypeName);
            var typeIndex = TypeManager.GetTypeIndex(type);
            var typeInfo = TypeManager.GetTypeInfo(typeIndex);
            data.StableTypeHash = typeInfo.StableTypeHash;
        }
    }
}
