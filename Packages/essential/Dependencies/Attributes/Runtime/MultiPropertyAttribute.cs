using System;
using System.Diagnostics;
using UnityEngine;

namespace EntitiesBT.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    [Conditional("UNITY_EDITOR")]
    public abstract class MultiPropertyAttribute : PropertyAttribute { }
}