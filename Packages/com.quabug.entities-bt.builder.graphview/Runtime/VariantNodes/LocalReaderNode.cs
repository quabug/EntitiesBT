using System;
using EntitiesBT.Variant;

namespace EntitiesBT
{
    public class LocalReaderNode : VariantNode<IVariantReader>
    {
        protected override string VariantTypeName => "Local";
        protected override Type BaseVariantGenericType => typeof(LocalVariant.Reader<>);
    }
}