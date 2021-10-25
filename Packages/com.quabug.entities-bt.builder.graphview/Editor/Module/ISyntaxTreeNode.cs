using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace EntitiesBT.Editor
{
    public interface ISyntaxTreeNode
    {
        Vector2 Position { get; set; }
        void SetParent([CanBeNull] ISyntaxTreeNode node);
        IEnumerable<ISyntaxTreeNode> Children { get; }
    }
}