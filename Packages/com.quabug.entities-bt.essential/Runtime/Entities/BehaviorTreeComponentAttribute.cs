using System;
using JetBrains.Annotations;
using Unity.Entities;

namespace EntitiesBT.Entities
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    [BaseTypeRequired(typeof(IComponentData))]
    public class BehaviorTreeComponentAttribute : Attribute {}
}
