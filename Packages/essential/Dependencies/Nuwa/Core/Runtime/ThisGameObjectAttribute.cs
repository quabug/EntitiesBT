using JetBrains.Annotations;
using UnityEngine;

namespace Nuwa
{
    [BaseTypeRequired(typeof(GameObject))]
    public class ThisGameObjectAttribute : MultiPropertyAttribute {}
}