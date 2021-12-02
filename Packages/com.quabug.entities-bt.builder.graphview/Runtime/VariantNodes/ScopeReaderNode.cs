using System;
using EntitiesBT.Variant;

namespace EntitiesBT
{
    public class ScopeReaderNode : VariantNode<IVariantReader>
    {
        protected override string VariantTypeName => "Scope";
        protected override Type BaseVariantGenericType => typeof(ScopeComponentVariant.Reader<>);
    }
}