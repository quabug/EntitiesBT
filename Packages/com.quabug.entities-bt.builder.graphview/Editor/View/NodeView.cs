using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace EntitiesBT.Editor
{
    public class NodeView : Node
    {
        public int Id { get; }
        public Vector2 Position => transform.position;

        public NodeView(in BehaviorTreeGraphNode node)
        {
            title = node.Title;
            Id = node.Id;
            viewDataKey = Id.ToString();

            style.left = node.Position.x;
            style.top = node.Position.y;
        }

        public sealed override string title
        {
            get => base.title;
            set => base.title = value;
        }
    }
}