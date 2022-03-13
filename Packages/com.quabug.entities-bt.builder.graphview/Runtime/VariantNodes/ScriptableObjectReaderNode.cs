using System;
using EntitiesBT.Variant;

namespace EntitiesBT
{
    public class ScriptableObjectReaderNode : VariantNode<IVariantReader>
    {
        protected override string VariantTypeName => "ScriptableObject";
        protected override Type BaseVariantGenericType => typeof(ScriptableObjectVariant.Reader<>);
    }
}