using System;
using EntitiesBT.Variant;

namespace EntitiesBT
{
    public class GlobalReaderAndWriterNode : VariantNode<IVariantReaderAndWriter>
    {
        protected override string VariantTypeName => "Global";
        protected override Type BaseVariantGenericType => typeof(GlobalComponentVariant.ReaderAndWriter<>);
    }
}