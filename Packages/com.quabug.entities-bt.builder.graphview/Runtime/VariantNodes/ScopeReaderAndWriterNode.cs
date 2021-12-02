using System;
using EntitiesBT.Variant;

namespace EntitiesBT
{
    public class ScopeReaderAndWriterNode : VariantNode<IVariantReaderAndWriter>
    {
        protected override string VariantTypeName => "Scope";
        protected override Type BaseVariantGenericType => typeof(ScopeComponentVariant.ReaderAndWriter<>);
    }
}