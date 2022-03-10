using System;
using EntitiesBT.Variant;

namespace EntitiesBT
{
    public class GlobalReaderNode : VariantNode<IVariantReader>
    {
        protected override string VariantTypeName => "Global";
        protected override Type BaseVariantGenericType => typeof(GlobalComponentVariant.Reader<>);
    }
}