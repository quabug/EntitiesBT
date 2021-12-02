using System;
using EntitiesBT.Variant;

namespace EntitiesBT
{
    public class ScopeWriterNode : VariantNode<IVariantWriter>
    {
        protected override string VariantTypeName => "Scope";
        protected override Type BaseVariantGenericType => typeof(ScopeComponentVariant.Writer<>);
    }
}