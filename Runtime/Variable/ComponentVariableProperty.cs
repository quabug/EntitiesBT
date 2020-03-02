using System;
using EntitiesBT.Core;
using Unity.Entities;
using UnityEngine;

namespace EntitiesBT.Variable
{
    public struct DynamicComponentData
    {
        public ulong StableHash;
        public int Offset;
    }
    
    [Serializable]
    public class ComponentVariableProperty<T> : VariableProperty<T> where T : struct
    {
        public enum AccessMode
        {
            ReadOnly,
            ReadWrite,
            Optional
        }
        
        public override int VariablePropertyTypeId => ID;
        public T FallbackValue;
        [VariableComponentData] public string ComponentValueName;
        public AccessMode Mode;
        
        protected override void AllocateData(ref BlobBuilder builder, ref BlobVariable<T> blobVariable)
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
            VariableRegisters<T>.Register(ID, GetData, GetDataRef);
        }

        public static readonly int ID = new Guid("4137908F-E81F-4D9C-8302-451421527330").GetHashCode();
        
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
    }
}
