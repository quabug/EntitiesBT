using System.Collections.Generic;
using System.IO;
using System.Linq;
using Blob;
using EntitiesBT.Core;
using JetBrains.Annotations;
using Unity.Entities;
using Unity.Entities.Serialization;

namespace EntitiesBT.Entities
{
    public static class NodeBlobExtensions
    {
        [Pure]
        public static BlobAssetReference<NodeBlob> ToBlob(
            [NotNull] this INodeDataBuilder root
            , IEnumerable<IGlobalValuesBuilder> globalValuesList
        )
        {
            return root.ToBuilder(globalValuesList).CreateUnityBlobAssetReference();
        }

        public static unsafe void SaveToStream(
            [NotNull] this INodeDataBuilder builder
            , [NotNull] IReadOnlyList<IGlobalValuesBuilder> scopeValuesList
            , [NotNull] Stream stream
        )
        {
            using var blob = builder.ToBlob(scopeValuesList);
            using var writer = new MemoryBinaryWriter();
            writer.Write(NodeBlob.VERSION);
            writer.Write(blob);
            var runtimePartSize = blob.Value.RuntimeSize;
            // HACK: truncate the runtime part of data (NodeState and RuntimeNodeData)
            var finalSize = writer.Length - runtimePartSize;
            using var writerData = new UnmanagedMemoryStream(writer.Data, finalSize);
            writerData.CopyTo(stream);
        }

        [Pure]
        public static Entity CloneBehaviorTree(this EntityManager dstManager, Entity behaviorTreeEntity)
        {
            var query = dstManager.GetSharedComponentData<BlackboardDataQuery>(behaviorTreeEntity);
            var comp = dstManager.GetComponentData<BehaviorTreeComponent>(behaviorTreeEntity);
            var clone = dstManager.CreateEntity();
#if UNITY_EDITOR
            dstManager.SetName(clone, dstManager.GetName(behaviorTreeEntity)+"-clone");
#endif
            dstManager.AddSharedComponentData(clone, query);
            var blob = new NodeBlobRef(comp.Blob.BlobRef.Clone());
            dstManager.AddComponentData(clone, new BehaviorTreeComponent(blob, comp.Thread, comp.AutoCreation));
            return clone;
        }

        [Pure]
        public static unsafe BlobAssetReference<T> Clone<T>(this BlobAssetReference<T> @this) where T : unmanaged
        {
            var untypedRef = BlobAssetReference.Create(@this);
            return BlobAssetReference<T>.Create(untypedRef.GetUnsafePtr(), untypedRef.Length);
        }
    }
}
