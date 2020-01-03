using System;
using Unity.Entities;

namespace EntitiesBT.Core
{
    public interface IBlackboard
    {
        object this[object key] { get; set; }
        bool Has(object key);
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
        
        public static void HasData<T>(this IBlackboard bb)
        {
            bb.Has(typeof(T));
        }

        public static bool IsComponentDataType(this Type type) =>
            type != null && type.IsValueType && typeof(IComponentData).IsAssignableFrom(type);
        
        public static bool IsManagedDataType(this Type type) =>
            type != null && type.IsClass && typeof(IComponentData).IsAssignableFrom(type);

        public static bool IsUnityComponentType(this Type type) =>
            type != null && type.IsSubclassOf(typeof(UnityEngine.Component));
    }
    
}
