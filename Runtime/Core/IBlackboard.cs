using Unity.Collections.LowLevel.Unsafe;

namespace EntitiesBT.Core
{
    public interface IBlackboard
    {
        object this[object key] { get; set; }
        bool Has(object key);
        unsafe void* GetPtr(object key);
    }

    public static class BlackboardExtensions
    {
        public static T GetData<T>(this IBlackboard bb)
        {
            return (T) bb[typeof(T)];
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
            return ref UnsafeUtilityEx.AsRef<T>(bb.GetPtr(typeof(T)));
        }
        
        public static unsafe ref T GetDataRef<T>(this IBlackboard bb, ulong componentStableHash, int componentDataOffset)
            where T : struct
        {
            var componentPtr = (byte*)bb.GetPtr(componentStableHash);
            // TODO: type safety check
            var dataPtr = componentPtr + componentDataOffset;
            return ref UnsafeUtilityEx.AsRef<T>(dataPtr);
        }
    }
}
