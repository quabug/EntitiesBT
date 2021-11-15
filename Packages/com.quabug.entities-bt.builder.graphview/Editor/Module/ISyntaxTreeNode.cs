using System;
using UnityEditor;
using UnityEngine;

namespace EntitiesBT.Editor
{
    public interface ISyntaxTreeNode : IDisposable
    {
        Vector2 Position { get; set; }

        SerializedProperty Name { get; }
    }
}