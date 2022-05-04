using System;
using System.Collections.Generic;
using Blob;
using EntitiesBT.Core;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using UnityEngine;
using static EntitiesBT.Core.Utilities;
using static EntitiesBT.Variant.Utilities;

namespace EntitiesBT.Variant
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class VariantComponentDataAttribute : PropertyAttribute {}

    [VariantClass(GUID)]
    public static class ComponentVariant
    {
        public const string GUID = "8E5CDB60-17DB-498A-B925-2094062769AB";

        [Serializable]
        public class Any<T> : IVariant where T : unmanaged
        {
            [VariantComponentData] public string ComponentValueName;

            public void Allocate(BlobVariantStream stream)
            {
                stream.SetVariantId(GuidHashCode(GUID));
                var data = GetTypeHashAndFieldOffset(ComponentValueName);
                if (data.Type != typeof(T) || data.Hash == 0)
                {
                    Debug.LogError($"{nameof(ComponentVariant)}({ComponentValueName}) is not valid, fallback to ConstantValue");
                    throw new ArgumentException();
                }
                stream.SetVariantValue(new DynamicComponentData { StableHash = data.Hash, Offset = data.Offset });
            }

            public object PreviewValue => throw new NotImplementedException();
        }

        [Serializable] public class Reader<T> : Any<T>, IVariantReader<T> where T : unmanaged {}
        [Serializable] public class Writer<T> : Any<T>, IVariantWriter<T> where T : unmanaged {}
        [Serializable] public class ReaderAndWriter<T> : Any<T>, IVariantReaderAndWriter<T> where T : unmanaged {}

        public struct DynamicComponentData
        {
            public ulong StableHash;
            public int Offset;
        }

        [AccessorMethod]
        private static IEnumerable<ComponentType> GetDynamicAccess(ref BlobVariant blobVariant)
        {
            ref var data = ref blobVariant.As<DynamicComponentData>();
            var typeIndex = TypeManager.GetTypeIndexFromStableTypeHash(data.StableHash);
            return ComponentType.FromTypeIndex(typeIndex).Yield();
        }

        private static IntPtr GetComponentValue(ulong stableHash, int offset, Func<Type, IntPtr> getDataPtr)
        {
            var index = TypeManager.GetTypeIndexFromStableTypeHash(stableHash);
            var componentPtr = getDataPtr(TypeManager.GetType(index));
            // TODO: type safety check
            var dataPtr = componentPtr + offset;
            return dataPtr;
        }

        private static unsafe ref T GetComponentValue<T>(ulong stableHash, int offset, Func<Type, IntPtr> getDataPtr) where T : unmanaged
        {
            var dataPtr = GetComponentValue(stableHash, offset, getDataPtr);
            return ref UnsafeUtility.AsRef<T>(dataPtr.ToPointer());
        }

        [ReaderMethod]
        private static T GetData<T, TNodeBlob, TBlackboard>(ref BlobVariant blobVariant, int index, ref TNodeBlob blob, ref TBlackboard bb)
            where T : unmanaged
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
            ref var data = ref blobVariant.As<DynamicComponentData>();
            return GetComponentValue<T>(data.StableHash, data.Offset, bb.GetDataPtrRO);
        }

        [RefReaderMethod]
        private static ref T GetDataRef<T, TNodeBlob, TBlackboard>(ref BlobVariant blobVariant, int index, ref TNodeBlob blob, ref TBlackboard bb)
            where T : unmanaged
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
            ref var data = ref blobVariant.As<DynamicComponentData>();
            return ref GetComponentValue<T>(data.StableHash, data.Offset, bb.GetDataPtrRW);
        }

        [ReadOnlyPointerMethod]
        private static IntPtr GetPointerRO<TNodeBlob, TBlackboard>(ref BlobVariant blobVariant, int index, ref TNodeBlob blob, ref TBlackboard bb)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
            ref var data = ref blobVariant.As<DynamicComponentData>();
            return GetComponentValue(data.StableHash, data.Offset, bb.GetDataPtrRO);
        }

        [ReadWritePointerMethod]
        private static IntPtr GetPointerRW<TNodeBlob, TBlackboard>(ref BlobVariant blobVariant, int index, ref TNodeBlob blob, ref TBlackboard bb)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
            ref var data = ref blobVariant.As<DynamicComponentData>();
            return GetComponentValue(data.StableHash, data.Offset, bb.GetDataPtrRW);
        }
    }
}
