using System;
using System.Collections.Generic;
using UnityEngine;

namespace EntitiesBT.Editor
{
    public interface IBehaviorTreeGraph : IEnumerable<IBehaviorTreeNode>
    {
        string Name { get; }
        IBehaviorTreeNode AddNode(Type nodeType, Vector2 position);
    }

}