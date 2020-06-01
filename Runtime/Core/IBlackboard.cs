using System;
using JetBrains.Annotations;
using Unity.Entities;

namespace EntitiesBT.Core
{
    public interface IBlackboard
    {
        bool HasData<T>() where T : struct;
        T GetData<T>() where T : struct;
        ref T GetDataRef<T>() where T : struct;
        IntPtr GetDataPtrRO(Type type);
        IntPtr GetDataPtrRW(Type type);
        
        T GetObject<T>() where T : class;
    }
    
    public static class BlackboardExtensions
    {
    //     [Pure]
    //     public static unsafe T GetData<TBlackboard, T>(this ref TBlackboard bb)
    //         where TBlackboard : struct, IBlackboard
    //     {
    //         var type = typeof(T);
    //         if (type.IsValueType && typeof(IComponentData).IsAssignableFrom(type))
    //         {
    //             var ptr = new IntPtr(bb.GetPtrRO(type));
    //             // GC free
    //             return Marshal.PtrToStructure<T>(ptr);
    //         }
    //         return (T) bb[type];
    //     }
    //     
    //     [Pure]
    //     public static unsafe T GetData<T, TBlackboard>(this ref TBlackboard bb, ulong componentStableHash, int componentDataOffset)
    //         where T : struct
    //         where TBlackboard : struct, IBlackboard
    //     {
    //         var componentPtr = (byte*)bb.GetPtrRO(componentStableHash);
    //         // TODO: type safety check
    //         var dataPtr = componentPtr + componentDataOffset;
    //         return UnsafeUtilityEx.AsRef<T>(dataPtr);
    //     }
    //     
    //     public static void SetData<T, TBlackboard>(this ref TBlackboard bb, T data)
    //         where TBlackboard : struct, IBlackboard
    //     {
    //         bb[typeof(T)] = data;
    //     }
    //     
    //     [Pure]
    //     public static bool HasData<T, TBlackboard>(this ref TBlackboard bb)
    //         where TBlackboard : struct, IBlackboard
    //     {
    //         return bb.Has(typeof(T));
    //     }
    //     
    //     [Pure]
    //     public static unsafe ref T GetDataRef<T, TBlackboard>(this ref TBlackboard bb) where T : struct
    //         where TBlackboard : struct, IBlackboard
    //     {
    //         return ref UnsafeUtilityEx.AsRef<T>(bb.GetPtrRW(typeof(T)));
    //     }
    //     
    //     [Pure]
    //     public static unsafe ref T GetDataRef<T, TBlackboard>(this ref TBlackboard bb, ulong componentStableHash, int componentDataOffset)
    //         where T : struct
    //         where TBlackboard : struct, IBlackboard
    //     {
    //         var componentPtr = (byte*)bb.GetPtrRW(componentStableHash);
    //         // TODO: type safety check
    //         var dataPtr = componentPtr + componentDataOffset;
    //         return ref UnsafeUtilityEx.AsRef<T>(dataPtr);
    //     }
    
        [Pure]
        public static int FetchTypeIndex([NotNull] this object key)
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
