using System;
using EntitiesBT.Variant;

namespace EntitiesBT
{
    public class ComponentReaderNode : VariantNode<IVariantReader>
    {
        protected override string VariantTypeName => "Component";
        protected override Type BaseVariantGenericType => typeof(ComponentVariant.Reader<>);
    }
}