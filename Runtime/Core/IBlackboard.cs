using System;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;

namespace EntitiesBT.Core
{
    public interface IBlackboard
    {
        object this[object key] { get; set; }
        bool Has(object key);
        ref T GetRef<T>(object key) where T : struct;
    }

    public static partial class BlackboardExtensions
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
            return ref bb.GetRef<T>(typeof(T));
        }
    }
}
