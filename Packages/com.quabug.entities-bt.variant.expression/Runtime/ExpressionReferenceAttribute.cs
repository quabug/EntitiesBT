using System;

namespace EntitiesBT.Variant.Expression
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class ExpressionReferenceAttribute : Attribute
    {
        public Type Type { get; }
        public ExpressionReferenceAttribute(Type type) => Type = type;
    }
}