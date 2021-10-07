using System;
using System.Collections.Generic;
using UnityEngine;

namespace EntitiesBT.Editor
{
    public interface IBehaviorTreeGraph : IDisposable
    {
        string Name { get; }
        IBehaviorTreeNode AddNode(Type nodeType, Vector2 position);
        IVariantNode AddVariant(Type variantType, Vector2 position);
        IEnumerable<IBehaviorTreeNode> RootNodes { get; }
    }

}