using System;

namespace EntitiesBT.Editor
{
    public class ConnectableVariant
    {
        public string Id { get; }
        public string Name { get; }
        public GraphNodeVariant.Any Variant { get; }
        public Type VariantType => Variant.GetType();
        public bool IsConnected => Variant.Node != null;
        public int VariantPortIndex => Variant.VariantPortIndex;
        public int SyntaxNodePortIndex => Variant.SyntaxNodePortIndex;
        public int SyntaxNodeId => Variant.Node.gameObject.GetInstanceID();

        public ConnectableVariant(GraphNodeVariant.Any variant, string id, string name)
        {
            Id = id;
            Variant = variant;
            Name = name;
        }
    }
}