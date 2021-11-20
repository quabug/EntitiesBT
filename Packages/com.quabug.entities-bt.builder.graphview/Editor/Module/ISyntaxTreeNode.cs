using System;
using UnityEngine;

namespace EntitiesBT.Editor
{
    public interface ISyntaxTreeNode : IDisposable, ISelectable, IConnectableVariantContainer
    {
        Vector2 Position { get; set; }
        string Name { get; }
        Type VariantType { get; }

        void Connect(ConnectableVariant variant);
        void Disconnect(ConnectableVariant variant);
    }
}