using JetBrains.Annotations;
using UnityEngine;

namespace Nuwa
{
    [BaseTypeRequired(typeof(Component))]
    public class ThisComponentAttribute : MultiPropertyAttribute {}
}