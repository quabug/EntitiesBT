using System;
using UnityEditor;
using UnityEngine;

namespace EntitiesBT.Editor
{
    public interface ISyntaxTreeNode : IDisposable, ISelectable
    {
        Vector2 Position { get; set; }
        string Name { get; }
        SerializedObject NodeObject { get; }
        Type VariantType { get; }
    }
}