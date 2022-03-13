using System;
using EntitiesBT.Variant;

namespace EntitiesBT
{
    public class ComponentWriterNode : VariantNode<IVariantWriter>
    {
        protected override string VariantTypeName => "Component";
        protected override Type BaseVariantGenericType => typeof(ComponentVariant.Writer<>);
    }
}