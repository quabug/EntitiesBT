using System;
using EntitiesBT.Variant;

namespace EntitiesBT
{
    public class NodeReaderNode : VariantNode<IVariantReader>
    {
        protected override string VariantTypeName => "Node";
        protected override Type BaseVariantGenericType => typeof(NodeVariant.Reader<>);
    }
}