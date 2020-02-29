using System;
using EntitiesBT.Core;
using Unity.Collections.LowLevel.Unsafe;

namespace EntitiesBT.Variable
{
    public struct DynamicComponentData
    {
        public ulong StableHash;
        public int Offset;
        public bool IsReadOnly;
    }
    //
    // public struct DynamicScriptableObjectData
    // {
    //     public Hash128 Id;
    //     public int Offset;
    // }

    public struct DynamicNodeData
    {
        public int Index;
        public int Offset;
    }
    
    public struct BlobVariable<T> where T : struct
    {
        public ValueSource Source;
        public int OffsetPtr;

        public ref T ConstantData => ref Value<T>();
        public ref DynamicComponentData ComponentData => ref Value<DynamicComponentData>();
        // public ref DynamicScriptableObjectData ScriptableObjectData => ref Value<DynamicScriptableObjectData>();
        public ref DynamicNodeData NodeData => ref Value<DynamicNodeData>();
        
        private unsafe ref TValue Value<TValue>() where TValue : struct
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            if(OffsetPtr == 0)
                throw new InvalidOperationException("The accessed BlobPtr hasn't been allocated.");
#endif
            fixed (int* thisPtr = &OffsetPtr)
            {
                return ref UnsafeUtilityEx.AsRef<TValue>((byte*)thisPtr + OffsetPtr);
            }
        }
        
        public T GetData(int index, INodeBlob blob, IBlackboard bb)
        {
            switch (Source)
            {
            case ValueSource.CustomConstant:
            case ValueSource.ComponentConstant:
            case ValueSource.ScriptableObjectConstant:
            case ValueSource.NodeConstant:
                return ConstantData;
            case ValueSource.ComponentDynamic:
                return bb.GetData<T>(ComponentData.StableHash, ComponentData.Offset);
            case ValueSource.NodeDynamic:
                throw new NotImplementedException();
            default:
                throw new ArgumentOutOfRangeException();
            }
        }
        
        public ref T GetDataRef(int index, INodeBlob blob, IBlackboard bb)
        {
            switch (Source)
            {
            case ValueSource.CustomConstant:
            case ValueSource.ComponentConstant:
            case ValueSource.ScriptableObjectConstant:
            case ValueSource.NodeConstant:
                return ref ConstantData;
            case ValueSource.ComponentDynamic:
                return ref bb.GetDataRef<T>(ComponentData.StableHash, ComponentData.Offset);
            case ValueSource.NodeDynamic:
                throw new NotImplementedException();
            default:
                throw new ArgumentOutOfRangeException();
            }
        }
        
        public void SetData(int index, INodeBlob blob, IBlackboard bb, T value)
        {
            GetDataRef(index, blob, bb) = value;
        }
    }
}
