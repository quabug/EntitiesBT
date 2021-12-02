using System;
using EntitiesBT.Variant;

namespace EntitiesBT
{
    public class ComponentReaderAndWriterNode : VariantNode<IVariantReaderAndWriter>
    {
        protected override string VariantTypeName => "Component";
        protected override Type BaseVariantGenericType => typeof(ComponentVariant.ReaderAndWriter<>);
    }
}