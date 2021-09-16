using UnityEngine;

namespace EntitiesBT.Editor
{
    public interface IBehaviorTreeNode
    {
        int Id { get; }
        string Name { get; }
        Vector2 Position { get; set; }
    }
}