using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace EntitiesBT.Editor
{
    public class NodeView : Node
    {
        public int Id { get; }

        private IBehaviorTreeNode _node;

        public NodeView(IBehaviorTreeNode node)
        {
            _node = node;

            title = node.Name;
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

        public override void SetPosition(Rect newPos)
        {
            base.SetPosition(newPos);
            _node.Position = newPos.position;
        }
    }
}