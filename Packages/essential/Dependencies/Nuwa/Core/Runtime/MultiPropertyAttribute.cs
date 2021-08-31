using System;
using System.Diagnostics;
using UnityEngine;

namespace Nuwa
{
    [AttributeUsage(AttributeTargets.Field)]
    [Conditional("UNITY_EDITOR")]
    public abstract class MultiPropertyAttribute : PropertyAttribute { }
}