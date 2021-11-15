using System;
using System.Collections.Generic;
using UnityEngine;

namespace EntitiesBT.Editor
{
    public interface IBehaviorTreeGraph : IDisposable
    {
        string Name { get; }

        IBehaviorTreeNode AddBehaviorNode(Type nodeType, Vector2 position);
        IEnumerable<IBehaviorTreeNode> BehaviorTreeRootNodes { get; }

        ISyntaxTreeNode AddSyntaxNode(Type variantBaseType, Vector2 position);
        IEnumerable<ISyntaxTreeNode> SyntaxTreeRootNodes { get; }
    }

}