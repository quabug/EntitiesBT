using System;
using System.IO;
using EntitiesBT.Core;
using JetBrains.Annotations;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace EntitiesBT.Editor
{
    public sealed class NodeView : Node, IDisposable
    {
        public int Id { get; }

        internal readonly IBehaviorTreeNode Node;
        internal Port Input { get; private set; }
        internal Port Output { get; private set; }

        public NodeView(IBehaviorTreeNode node)
            : base(Path.Combine(Utilities.GetCurrentDirectoryProjectRelativePath(), "NodeView.uxml"))
        {
            Node = node;

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
                Input = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(NodeView));
                Input.portName = "";
                inputContainer.Add(Input);
            }

            void CreateOutputPort(Port.Capacity capacity)
            {
                Output = InstantiatePort(Orientation.Vertical, Direction.Output, capacity, typeof(NodeView));
                Output.portName = "";
                outputContainer.Add(Output);
            }
        }

        public void Dispose()
        {
            Node.Dispose();
        }

        public void ConnectTo([NotNull] NodeView child)
        {
            child.Node.SetParent(Node);
        }

        public void DisconnectFrom([NotNull] NodeView parent)
        {
            Node.SetParent(null);
        }

        public override void OnSelected()
        {
            base.OnSelected();
            Node.OnSelected();
        }

        public override void OnUnselected()
        {
            base.OnUnselected();
            Node.OnUnselected();
        }

        public void SyncPosition()
        {
            Node.Position = GetPosition().position;
        }
    }
}