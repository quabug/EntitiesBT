using System;
using EntitiesBT.Core;
using Unity.Collections.LowLevel.Unsafe;

namespace EntitiesBT.Variable
{
    // HACK: generic struct with explicit layout would throw exception on `Assembly.GetTypes`???
    // [StructLayout(LayoutKind.Explicit), MayOnlyLiveInBlobStorage, Serializable]
    public struct BlobVariable<T> where T : struct
    {
    //     [FieldOffset(0)] public VariableValueSource Source;
    //     [FieldOffset(4)] public BlobPtr<T> ConstantData;
    //     [FieldOffset(4)] public BlobPtr<DynamicComponentData> ComponentData;
    //     [FieldOffset(4)] public BlobPtr<DynamicScriptableObjectData> ScriptableObjectData;
    //     [FieldOffset(4)] public BlobPtr<DynamicNodeData> NodeData;
        public ValueSource Source;
        public int OffsetPtr;

        public ref T ConstantData => ref Value<T>();
        public ref DynamicComponentData ComponentData => ref Value<DynamicComponentData>();
        public ref DynamicScriptableObjectData ScriptableObjectData => ref Value<DynamicScriptableObjectData>();
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
            case ValueSource.Constant:
            case ValueSource.ConstantComponent:
            case ValueSource.ConstantScriptableObject:
            case ValueSource.ConstantNode:
                return ConstantData;
            case ValueSource.DynamicComponent:
                return bb.GetData<T>(ComponentData.StableHash, ComponentData.Offset);
            case ValueSource.DynamicScriptableObject:
                throw new NotImplementedException();
            case ValueSource.DynamicNode:
                throw new NotImplementedException();
            default:
                throw new ArgumentOutOfRangeException();
            }
        }
        
        public ref T GetDataRef(int index, INodeBlob blob, IBlackboard bb)
        {
            switch (Source)
            {
            case ValueSource.Constant:
            case ValueSource.ConstantComponent:
            case ValueSource.ConstantScriptableObject:
            case ValueSource.ConstantNode:
                return ref ConstantData;
            case ValueSource.DynamicComponent:
                return ref bb.GetDataRef<T>(ComponentData.StableHash, ComponentData.Offset);
            case ValueSource.DynamicScriptableObject:
                throw new NotImplementedException();
            case ValueSource.DynamicNode:
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
