using System;
using EntitiesBT.Variant;

namespace EntitiesBT
{
    public class LocalReaderAndWriterNode : VariantNode<IVariantReaderAndWriter>
    {
        protected override string VariantTypeName => "Local";
        protected override Type BaseVariantGenericType => typeof(LocalVariant.ReaderAndWriter<>);
    }
}