using System;
using UnityEngine;

namespace EntitiesBT.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class MultiPropertyAttribute : PropertyAttribute { }

    [AttributeUsage(AttributeTargets.Field)]
    public abstract class MultiPropertyDecoratorAttribute : Attribute
    {
        public int Order;
    }
}