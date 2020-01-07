using System;
using Unity.Entities;

namespace Entities
{
    delegate object GetDataDelegate(object caller, Type dataType);
    delegate void SetDataDelegate(object caller, Type dataType, object dataValue);
    delegate bool HasDataDelegate(object caller, Type dataType);
    
    public static partial class BlackboardExtensions
    {
        public static bool IsComponentDataType(this Type type) =>
            type != null && type.IsValueType && typeof(IComponentData).IsAssignableFrom(type);
        
        public static bool IsSharedComponentDataType(this Type type) =>
            type != null && type.IsValueType && typeof(ISharedComponentData).IsAssignableFrom(type);
        
        public static bool IsManagedDataType(this Type type) =>
            type != null && type.IsClass && typeof(IComponentData).IsAssignableFrom(type);

        public static bool IsUnityComponentType(this Type type) =>
            type != null && type.IsSubclassOf(typeof(UnityEngine.Component));
    }
}
