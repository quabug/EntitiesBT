using System;
using EntitiesBT.Variant;

namespace EntitiesBT
{
    public class LocalWriterNode : VariantNode<IVariantWriter>
    {
        protected override string VariantTypeName => "Local";
        protected override Type BaseVariantGenericType => typeof(LocalVariant.Writer<>);
    }
}