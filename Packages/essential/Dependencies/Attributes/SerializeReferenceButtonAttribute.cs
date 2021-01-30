using System;

namespace EntitiesBT.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public class SerializeReferenceButtonAttribute : MultiPropertyDecoratorAttribute
    {
        public SerializeReferenceButtonAttribute() => Order = int.MaxValue;
    }
}
