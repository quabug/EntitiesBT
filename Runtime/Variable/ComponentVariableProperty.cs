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
        public bool IsReadOnly;
    }
    
    [Serializable]
    public class ComponentVariableProperty<T> : VariableProperty<T> where T : struct
    {
        public T FallbackValue;
        public string ComponentTypeName;
        public string ComponentValueName;

        public override void Allocate(ref BlobBuilder builder, ref BlobVariable<T> blobVariable)
        {
            blobVariable.VariableId = ID;
            var (hash, offset, valueType) = Utility.GetTypeHashAndFieldOffset(ComponentTypeName, ComponentValueName);
            if (valueType != typeof(T) || hash == 0)
            {
                Debug.LogError($"ComponentVariable({ComponentTypeName}.{ComponentValueName}) is not valid, fallback to ConstantValue");
                builder.Allocate(ref blobVariable, FallbackValue);
                return;
            }
            builder.Allocate(ref blobVariable, new DynamicComponentData{StableHash = hash, Offset = offset});
        }
        
        static ComponentVariableProperty()
        {
            VariableRegisters<T>.Register(ID, GetData, GetDataRef);
        }

        public static int ID = new Guid("4137908F-E81F-4D9C-8302-451421527330").GetHashCode();
        
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
