using System;
using System.Collections.Generic;
using EntitiesBT.Core;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Scripting;
using static EntitiesBT.Core.Utilities;
using static EntitiesBT.Variant.Utilities;

namespace EntitiesBT.Variant
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class VariantComponentDataAttribute : PropertyAttribute {}

    public static class ComponentVariant
    {
        public const string GUID = "8E5CDB60-17DB-498A-B925-2094062769AB";

        public struct DynamicComponentData
        {
            public ulong StableHash;
            public int Offset;
        }

        [Preserve, AccessorMethod(GUID)]
        private static IEnumerable<ComponentType> GetDynamicAccess(ref BlobVariant blobVariant)
        {
            var hash = blobVariant.Value<DynamicComponentData>().StableHash;
            var typeIndex = TypeManager.GetTypeIndexFromStableTypeHash(hash);
            return ComponentType.FromTypeIndex(typeIndex).Yield();
        }

        [Serializable]
        public class Reader<T> : IVariantReader<T> where T : struct
        {
            [VariantComponentData] public string ComponentValueName;

            public void Allocate(ref BlobBuilder builder, ref BlobVariant blobVariant, INodeDataBuilder self, ITreeNode<INodeDataBuilder>[] tree)
            {
                blobVariant.VariantId = GuidHashCode(GUID);
                var data = GetTypeHashAndFieldOffset(ComponentValueName);
                if (data.Type != typeof(T) || data.Hash == 0)
                {
                    Debug.LogError($"ComponentVariant({ComponentValueName}) is not valid, fallback to ConstantValue", (UnityEngine.Object)self);
                    throw new ArgumentException();
                }
                builder.Allocate(ref blobVariant, new DynamicComponentData{StableHash = data.Hash, Offset = data.Offset});
            }

            private static unsafe ref T GetComponentValue(ulong stableHash, int offset, Func<Type, IntPtr> getDataPtr)
            {
                var index = TypeManager.GetTypeIndexFromStableTypeHash(stableHash);
                var componentPtr = getDataPtr(TypeManager.GetType(index));
                // TODO: type safety check
                var dataPtr = componentPtr + offset;
                return ref UnsafeUtility.AsRef<T>(dataPtr.ToPointer());
            }

            [Preserve, ReaderMethod(GUID)]
            private static T GetData<TNodeBlob, TBlackboard>(ref BlobVariant blobVariant, int index, ref TNodeBlob blob, ref TBlackboard bb)
                where TNodeBlob : struct, INodeBlob
                where TBlackboard : struct, IBlackboard
            {
                ref var data = ref blobVariant.Value<DynamicComponentData>();
                return GetComponentValue(data.StableHash, data.Offset, bb.GetDataPtrRO);
            }

            [Preserve, ReaderMethod(GUID)]
            private static ref T GetDataRef<TNodeBlob, TBlackboard>(ref BlobVariant blobVariant, int index, ref TNodeBlob blob, ref TBlackboard bb)
                where TNodeBlob : struct, INodeBlob
                where TBlackboard : struct, IBlackboard
            {
                ref var data = ref blobVariant.Value<DynamicComponentData>();
                return ref GetComponentValue(data.StableHash, data.Offset, bb.GetDataPtrRW);
            }
        }

        public class Writer<T> : IVariantWriter<T> where T : unmanaged
        {
            public void Allocate(ref BlobBuilder builder, ref BlobVariant blobVariant, INodeDataBuilder self, ITreeNode<INodeDataBuilder>[] tree)
            {
                throw new NotImplementedException();
            }
        }
    }
}
