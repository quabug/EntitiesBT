using System;
using EntitiesBT.Variant;

namespace EntitiesBT
{
    public class NodeWriterNode : VariantNode<IVariantWriter>
    {
        protected override string VariantTypeName => "Node";
        protected override Type BaseVariantGenericType => typeof(NodeVariant.Writer<>);
    }
}