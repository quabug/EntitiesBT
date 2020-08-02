using System;
using UnityEngine;

namespace EntitiesBT.Components
{
    [AttributeUsage(AttributeTargets.Field)]
    public class SerializedTypeAttribute : PropertyAttribute
    {
        public Type BaseType;
        public SerializedTypeAttribute(Type baseType = null) => BaseType = baseType;
    }
}
