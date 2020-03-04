using System;
using System.Collections.Generic;
using EntitiesBT.Core;
using EntitiesBT.Variable;
using Unity.Entities;
using UnityEngine;

namespace EntitiesBT.Components
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class VariableComponentDataAttribute : PropertyAttribute {}
    
    public struct DynamicComponentData
    {
        public ulong StableHash;
        public int Offset;
    }
    
    [Serializable]
    public class ComponentVariableProperty<T> : VariableProperty<T> where T : struct
    {
        private enum AccessMode
        {
            ReadOnly,
            ReadWrite,
            Optional
        }
        
        public override int VariablePropertyTypeId
        {
            get
            {
                switch (_accessMode)
                {
                case AccessMode.ReadOnly:
                    return _ID_READ_ONLY;
                case AccessMode.ReadWrite:
                    return _ID_READ_WRITE;
                case AccessMode.Optional:
                    return _ID_OPTIONAL;
                default:
                    throw new ArgumentOutOfRangeException();
                }
            }
        }

        public T FallbackValue;
        [VariableComponentData] public string ComponentValueName;
        [SerializeField] private AccessMode _accessMode;
        
        protected override void AllocateData(ref BlobBuilder builder, ref BlobVariable<T> blobVariable, INodeDataBuilder self, ITreeNode<INodeDataBuilder>[] tree)
        {
            var data = Utility.GetTypeHashAndFieldOffset(ComponentValueName);
            if (data.Type != typeof(T) || data.Hash == 0)
            {
                Debug.LogError($"ComponentVariable({ComponentValueName}) is not valid, fallback to ConstantValue");
                builder.Allocate(ref blobVariable, FallbackValue);
                return;
            }
            builder.Allocate(ref blobVariable, new DynamicComponentData{StableHash = data.Hash, Offset = data.Offset});
        }

        static ComponentVariableProperty()
        {
            VariableRegisters<T>.Register(_ID_READ_ONLY, GetData, null, GetReadOnlyAccess);
            VariableRegisters<T>.Register(_ID_READ_WRITE, GetData, GetDataRef, GetReadWriteAccess);
            VariableRegisters<T>.Register(_ID_OPTIONAL, GetData, GetDataRef);
        }

        private static readonly int _ID_READ_ONLY = new Guid("4137908F-E81F-4D9C-8302-451421527330").GetHashCode();
        private static readonly int _ID_READ_WRITE = new Guid("8E5CDB60-17DB-498A-B925-2094062769AB").GetHashCode();
        private static readonly int _ID_OPTIONAL = new Guid("8F83A82E-BABE-437D-B5E7-E2604C2F9ABA").GetHashCode();
        
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
        
        private static IEnumerable<ComponentType> GetReadOnlyAccess(ref BlobVariable<T> blobVariable)
        {
            var hash = blobVariable.Value<DynamicComponentData>().StableHash;
            var typeIndex = TypeManager.GetTypeIndexFromStableTypeHash(hash);
            return ComponentType.ReadOnly(typeIndex).Yield();
        }
        
        private static IEnumerable<ComponentType> GetReadWriteAccess(ref BlobVariable<T> blobVariable)
        {
            var hash = blobVariable.Value<DynamicComponentData>().StableHash;
            var typeIndex = TypeManager.GetTypeIndexFromStableTypeHash(hash);
            return ComponentType.FromTypeIndex(typeIndex).Yield();
        }
    }
}
