using System;
using UnityEngine;

namespace EntitiesBT.Editor
{
    public interface ISyntaxTreeNode : IDisposable, ISelectable, IConnectableVariantContainer
    {
        int Id { get; }
        Vector2 Position { get; set; }
        string Name { get; }
        Type VariantType { get; }

        void Connect(ConnectableVariant variant, int variantPortIndex, int syntaxNodePortIndex);
        void Disconnect(ConnectableVariant variant);
    }
}