using System;
using EntitiesBT.Variant;

namespace EntitiesBT
{
    public class NodeReaderAndWriterNode : VariantNode<IVariantReaderAndWriter>
    {
        protected override string VariantTypeName => "Node";
        protected override Type BaseVariantGenericType => typeof(NodeVariant.ReaderAndWriter<>);
    }
}