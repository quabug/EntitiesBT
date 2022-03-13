using System;
using EntitiesBT.Variant;

namespace EntitiesBT
{
    public class GlobalWriterNode : VariantNode<IVariantWriter>
    {
        protected override string VariantTypeName => "Global";
        protected override Type BaseVariantGenericType => typeof(GlobalComponentVariant.Writer<>);
    }
}