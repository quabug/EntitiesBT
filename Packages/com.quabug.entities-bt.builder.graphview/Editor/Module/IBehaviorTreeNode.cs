using System;
using EntitiesBT.Core;
using UnityEngine;

namespace EntitiesBT.Editor
{
    public interface IBehaviorTreeNode : IDisposable
    {
        int Id { get; }
        string Name { get; }
        Vector2 Position { get; set; }
        BehaviorNodeType NodeType { get; }
    }
}