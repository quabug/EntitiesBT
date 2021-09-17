using System;
using System.IO;
using EntitiesBT.Core;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace EntitiesBT.Editor
{
    public sealed class NodeView : Node, IDisposable
    {
        public int Id { get; }

        private readonly IBehaviorTreeNode _node;

        public NodeView(IBehaviorTreeNode node)
            : base(Path.Combine(Utilities.GetCurrentDirectoryProjectRelativePath(), "NodeView.uxml"))
        {
            _node = node;

            title = node.Name;
            if (title.EndsWith("Node")) title = title.Substring(0, title.Length - "Node".Length);

            Id = node.Id;
            viewDataKey = Id.ToString();

            style.left = node.Position.x;
            style.top = node.Position.y;

            CreateInputPort();
            switch (node.BehaviorType)
            {
                case BehaviorNodeType.Composite:
                    CreateOutputPort(Port.Capacity.Multi);
                    break;
                case BehaviorNodeType.Decorate:
                    CreateOutputPort(Port.Capacity.Single);
                    break;
                case BehaviorNodeType.Action:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            AddToClassList(node.BehaviorType.ToString().ToLower());
            AddToClassList(node.NodeType.Name);

            void CreateInputPort()
            {
                var port = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(NodeView));
                port.portName = "";
                inputContainer.Add(port);
            }

            void CreateOutputPort(Port.Capacity capacity)
            {
                var port = InstantiatePort(Orientation.Vertical, Direction.Output, capacity, typeof(NodeView));
                port.portName = "";
                outputContainer.Add(port);
            }
        }

        public override void SetPosition(Rect newPos)
        {
            base.SetPosition(newPos);
            _node.Position = newPos.position;
        }

        public void Dispose()
        {
            _node.Dispose();
        }

        public void SetParent(int parentNodeId)
        {

        }

        public override void OnSelected()
        {
            _node.OnSelected();
        }
    }
}