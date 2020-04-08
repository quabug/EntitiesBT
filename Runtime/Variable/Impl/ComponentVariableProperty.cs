using System;
using System.Collections.Generic;
using EntitiesBT.Core;
using Unity.Entities;
using UnityEngine;

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
            VariableRegisters<T>.Register(_DYNAMIC_ID, GetData, GetDataRef, GetDynamicAccess);
            VariableRegisters<T>.Register(_COPYTOLOCAL_ID, CopyAndGetData, CopyAndGetDataRef, GetCopyToLocalAccess);
            VariableRegisters<T>.Register(_LOCAL_ID, GetLocalData, GetLocalDataRef, GetCopyToLocalAccess);
        }

        private static readonly int _DYNAMIC_ID = new Guid("8E5CDB60-17DB-498A-B925-2094062769AB").GetHashCode();
        private static readonly int _COPYTOLOCAL_ID = new Guid("F89F8ACD-1D27-4253-8BEB-8411FB3D6773").GetHashCode();
        private static readonly int _LOCAL_ID = new Guid("48E4C6C0-F715-48DD-9CD8-E5DB6C940B5C").GetHashCode();
        
        private static T GetData(ref BlobVariable<T> blobVariable, int index, INodeBlob blob, IBlackboard bb)
        {
            ref var data = ref blobVariable.Value<DynamicComponentData>();
            return bb.GetData<T>(data.StableHash, data.Offset);
        }
        
        private static ref T GetDataRef(ref BlobVariable<T> blobVariable, int index, INodeBlob blob, IBlackboard bb)
        {
            ref var data = ref blobVariable.Value<DynamicComponentData>();
            return ref bb.GetDataRef<T>(data.StableHash, data.Offset);
        }
        
        private static T CopyAndGetData(ref BlobVariable<T> blobVariable, int index, INodeBlob blob, IBlackboard bb)
        {
            blobVariable.VariableId = _LOCAL_ID;
            ref var data = ref blobVariable.Value<CopyToLocalComponentData>();
            data.LocalValue = bb.GetData<T>(data.StableHash, data.Offset);
            return data.LocalValue;
        }
        
        private static ref T CopyAndGetDataRef(ref BlobVariable<T> blobVariable, int index, INodeBlob blob, IBlackboard bb)
        {
            blobVariable.VariableId = _LOCAL_ID;
            ref var data = ref blobVariable.Value<CopyToLocalComponentData>();
            data.LocalValue = bb.GetDataRef<T>(data.StableHash, data.Offset);
            return ref data.LocalValue;
        }
        
        private static T GetLocalData(ref BlobVariable<T> blobVariable, int index, INodeBlob blob, IBlackboard bb)
        {
            ref var data = ref blobVariable.Value<CopyToLocalComponentData>();
            return data.LocalValue;
        }
        
        private static ref T GetLocalDataRef(ref BlobVariable<T> blobVariable, int index, INodeBlob blob, IBlackboard bb)
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
