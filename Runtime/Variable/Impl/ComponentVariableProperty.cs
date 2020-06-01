using System;
using System.Collections.Generic;
using EntitiesBT.Core;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Scripting;

namespace EntitiesBT.Variable
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class VariableComponentDataAttribute : PropertyAttribute {}
    
    [Serializable]
    public class ComponentVariableProperty<T> : VariableProperty<T> where T : struct
    {
        private struct DynamicComponentData
        {
            public ulong StableHash;
            public int Offset;
        }
        
        private struct CopyToLocalComponentData
        {
            public ulong StableHash;
            public int Offset;
            public T LocalValue;
        }
        
        public override int VariablePropertyTypeId => CopyToLocalNode ? _COPYTOLOCAL_ID : _DYNAMIC_ID;

        [VariableComponentData] public string ComponentValueName;

        [Tooltip("Will read component data into local node and never write back into component data. (Force `ReadOnly` access)")]
        public bool CopyToLocalNode;
        
        protected override void AllocateData(ref BlobBuilder builder, ref BlobVariable<T> blobVariable, INodeDataBuilder self, ITreeNode<INodeDataBuilder>[] tree)
        {
            var data = Utility.GetTypeHashAndFieldOffset(ComponentValueName);
            if (data.Type != typeof(T) || data.Hash == 0)
            {
                Debug.LogError($"ComponentVariable({ComponentValueName}) is not valid, fallback to ConstantValue", (UnityEngine.Object)self);
                throw new ArgumentException();
            }
            if (CopyToLocalNode) builder.Allocate(ref blobVariable, new CopyToLocalComponentData{StableHash = data.Hash, Offset = data.Offset, LocalValue = default});
            else builder.Allocate(ref blobVariable, new DynamicComponentData{StableHash = data.Hash, Offset = data.Offset});
        }

        static ComponentVariableProperty()
        {
            var type = typeof(ComponentVariableProperty<T>);
            VariableRegisters<T>.Register(_DYNAMIC_ID, type.Getter("GetData"), type.Getter("GetDataRef"), GetDynamicAccess);
            VariableRegisters<T>.Register(_COPYTOLOCAL_ID, type.Getter("CopyAndGetData"), type.Getter("CopyAndGetDataRef"), GetCopyToLocalAccess);
            VariableRegisters<T>.Register(_LOCAL_ID, type.Getter("GetLocalData"), type.Getter("GetLocalDataRef"), GetCopyToLocalAccess);
        }

        private static readonly int _DYNAMIC_ID = new Guid("8E5CDB60-17DB-498A-B925-2094062769AB").GetHashCode();
        private static readonly int _COPYTOLOCAL_ID = new Guid("F89F8ACD-1D27-4253-8BEB-8411FB3D6773").GetHashCode();
        private static readonly int _LOCAL_ID = new Guid("48E4C6C0-F715-48DD-9CD8-E5DB6C940B5C").GetHashCode();

        [Preserve]
        private static unsafe ref T GetComponentValue(ulong stableHash, int offset, Func<Type, IntPtr> getDataPtr)
        {
            var index = TypeManager.GetTypeIndexFromStableTypeHash(stableHash);
            var componentPtr = getDataPtr(TypeManager.GetType(index));
            // TODO: type safety check
            var dataPtr = componentPtr + offset;
            return ref UnsafeUtilityEx.AsRef<T>(dataPtr.ToPointer());
        }
        
        [Preserve]
        private static T GetData<TNodeBlob, TBlackboard>(ref BlobVariable<T> blobVariable, int index, ref TNodeBlob blob, ref TBlackboard bb)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
            ref var data = ref blobVariable.Value<DynamicComponentData>();
            return GetComponentValue(data.StableHash, data.Offset, bb.GetDataPtrRO);
        }
        
        [Preserve]
        private static ref T GetDataRef<TNodeBlob, TBlackboard>(ref BlobVariable<T> blobVariable, int index, ref TNodeBlob blob, ref TBlackboard bb)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
            ref var data = ref blobVariable.Value<DynamicComponentData>();
            return ref GetComponentValue(data.StableHash, data.Offset, bb.GetDataPtrRW);
        }
        
        [Preserve]
        private static T CopyAndGetData<TNodeBlob, TBlackboard>(ref BlobVariable<T> blobVariable, int index, ref TNodeBlob blob, ref TBlackboard bb)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
            blobVariable.VariableId = _LOCAL_ID;
            ref var data = ref blobVariable.Value<CopyToLocalComponentData>();
            data.LocalValue = GetComponentValue(data.StableHash, data.Offset, bb.GetDataPtrRO);
            return data.LocalValue;
        }
        
        [Preserve]
        private static ref T CopyAndGetDataRef<TNodeBlob, TBlackboard>(ref BlobVariable<T> blobVariable, int index, ref TNodeBlob blob, ref TBlackboard bb)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
            blobVariable.VariableId = _LOCAL_ID;
            ref var data = ref blobVariable.Value<CopyToLocalComponentData>();
            data.LocalValue = GetComponentValue(data.StableHash, data.Offset, bb.GetDataPtrRO);
            return ref data.LocalValue;
        }
        
        [Preserve]
        private static T GetLocalData<TNodeBlob, TBlackboard>(ref BlobVariable<T> blobVariable, int index, ref TNodeBlob blob, ref TBlackboard bb)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
            ref var data = ref blobVariable.Value<CopyToLocalComponentData>();
            return data.LocalValue;
        }
        
        [Preserve]
        private static ref T GetLocalDataRef<TNodeBlob, TBlackboard>(ref BlobVariable<T> blobVariable, int index, ref TNodeBlob blob, ref TBlackboard bb)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
            ref var data = ref blobVariable.Value<CopyToLocalComponentData>();
            return ref data.LocalValue;
        }
        
        private static IEnumerable<ComponentType> GetCopyToLocalAccess(ref BlobVariable<T> blobVariable)
        {
            var hash = blobVariable.Value<CopyToLocalComponentData>().StableHash;
            var typeIndex = TypeManager.GetTypeIndexFromStableTypeHash(hash);
            return ComponentType.ReadOnly(typeIndex).Yield();
        }
        
        private static IEnumerable<ComponentType> GetDynamicAccess(ref BlobVariable<T> blobVariable)
        {
            var hash = blobVariable.Value<DynamicComponentData>().StableHash;
            var typeIndex = TypeManager.GetTypeIndexFromStableTypeHash(hash);
            return ComponentType.FromTypeIndex(typeIndex).Yield();
        }
    }
}
