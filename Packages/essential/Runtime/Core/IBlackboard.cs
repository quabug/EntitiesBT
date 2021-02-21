using System;
using Unity.Entities;

namespace EntitiesBT.Core
{
    public interface IBlackboard
    {
        bool HasData<T>() where T : struct;
        T GetData<[ReadOnly(typeof(int))] T>() where T : struct;
        ref T GetDataRef<[ReadWrite] T>() where T : struct;

        bool HasData(Type type);
        IntPtr GetDataPtrRO([ReadOnly] Type type);
        IntPtr GetDataPtrRW([ReadWrite] Type type);
        
        T GetObject<[ReadWrite] T>() where T : class;
    }
    
    public static class BlackboardExtensions
    {
        public static DynamicBufferUnsafe<T> GetDynamicBufferUnsafeRO<[ReadOnly] T>(this IBlackboard blackboard) where T : struct, IBufferElementData
        {
            return blackboard.GetDataPtrRO(typeof(T)).ToDynamicBufferUnsafe<T>();
        }
        
        public static DynamicBufferUnsafe<T> GetDynamicBufferUnsafeRW<[ReadWrite] T>(this IBlackboard blackboard) where T : struct, IBufferElementData
        {
            return blackboard.GetDataPtrRW(typeof(T)).ToDynamicBufferUnsafe<T>();
        }

        private static DynamicBufferUnsafe<T> ToDynamicBufferUnsafe<T>(this IntPtr ptr) where T : struct, IBufferElementData
        {
            var internalCapacity = TypeManager.GetTypeInfo<T>().BufferCapacity;
            return new DynamicBufferUnsafe<T>(ptr, internalCapacity);
        }
            
    }
}
