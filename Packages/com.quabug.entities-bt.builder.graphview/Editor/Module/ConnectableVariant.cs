using System;

namespace EntitiesBT.Editor
{
    public class ConnectableVariant
    {
        public string Id { get; }
        public GraphNodeVariant.Any Variant { get; }
        public Type VariantType => Variant.GetType();

        public ConnectableVariant(GraphNodeVariant.Any variant, string id)
        {
            Id = id;
            Variant = variant;
        }
    }
}