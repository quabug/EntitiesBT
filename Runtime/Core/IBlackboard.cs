using System;
using System.Runtime.InteropServices;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;

namespace EntitiesBT.Core
{
    public interface IBlackboard
    {
        object this[object key] { get; set; }
        bool Has(object key);
        unsafe void* GetPtrRW(object key);
        unsafe void* GetPtrRO(object key);
    }

    public static class BlackboardExtensions
    {
        public static unsafe T GetData<T>(this IBlackboard bb)
        {
            var type = typeof(T);
            if (type.IsValueType && typeof(IComponentData).IsAssignableFrom(type))
            {
                var ptr = new IntPtr(bb.GetPtrRO(type));
                // GC free
                return Marshal.PtrToStructure<T>(ptr);
            }
            return (T) bb[type];
        }
        
        public static unsafe T GetData<T>(this IBlackboard bb, ulong componentStableHash, int componentDataOffset)
            where T : struct
        {
            var componentPtr = (byte*)bb.GetPtrRO(componentStableHash);
            // TODO: type safety check
            var dataPtr = componentPtr + componentDataOffset;
            return UnsafeUtilityEx.AsRef<T>(dataPtr);
        }
        
        public static void SetData<T>(this IBlackboard bb, T data)
        {
            bb[typeof(T)] = data;
        }
        
        public static bool HasData<T>(this IBlackboard bb)
        {
            return bb.Has(typeof(T));
        }
        
        public static unsafe ref T GetDataRef<T>(this IBlackboard bb) where T : struct
        {
            return ref UnsafeUtilityEx.AsRef<T>(bb.GetPtrRW(typeof(T)));
        }
        
        public static unsafe ref T GetDataRef<T>(this IBlackboard bb, ulong componentStableHash, int componentDataOffset)
            where T : struct
        {
            var componentPtr = (byte*)bb.GetPtrRW(componentStableHash);
            // TODO: type safety check
            var dataPtr = componentPtr + componentDataOffset;
            return ref UnsafeUtilityEx.AsRef<T>(dataPtr);
        }

        public static int FetchTypeIndex(this object key)
        {
            switch (key)
            {
            case Type type:
                return TypeManager.GetTypeIndex(type);
            case ulong hash:
                return TypeManager.GetTypeIndexFromStableTypeHash(hash);
            case int typeIndex:
                return typeIndex;
            default:
                throw new NotImplementedException();
            }
        }
    }
}
