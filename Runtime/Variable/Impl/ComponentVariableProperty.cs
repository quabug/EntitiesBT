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
        
        public override int VariablePropertyTypeId => _ID;

        [VariableComponentData] public string ComponentValueName;
        
        protected override void AllocateData(ref BlobBuilder builder, ref BlobVariable<T> blobVariable, INodeDataBuilder self, ITreeNode<INodeDataBuilder>[] tree)
        {
            var data = Utility.GetTypeHashAndFieldOffset(ComponentValueName);
            if (data.Type != typeof(T) || data.Hash == 0)
            {
                Debug.LogError($"ComponentVariable({ComponentValueName}) is not valid, fallback to ConstantValue", (UnityEngine.Object)self);
                throw new ArgumentException();
            }
            builder.Allocate(ref blobVariable, new DynamicComponentData{StableHash = data.Hash, Offset = data.Offset});
        }

        static ComponentVariableProperty()
        {
            VariableRegisters<T>.Register(_ID, GetData, GetDataRef, GetReadWriteAccess);
        }

        private static readonly int _ID = new Guid("8E5CDB60-17DB-498A-B925-2094062769AB").GetHashCode();
        
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
        
        private static IEnumerable<ComponentType> GetReadWriteAccess(ref BlobVariable<T> blobVariable)
        {
            var hash = blobVariable.Value<DynamicComponentData>().StableHash;
            var typeIndex = TypeManager.GetTypeIndexFromStableTypeHash(hash);
            return ComponentType.FromTypeIndex(typeIndex).Yield();
        }
    }
}
