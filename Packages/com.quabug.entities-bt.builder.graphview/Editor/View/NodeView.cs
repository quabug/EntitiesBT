using System;
using System.IO;
using EntitiesBT.Core;
using JetBrains.Annotations;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace EntitiesBT.Editor
{
    public sealed class NodeView : Node, IDisposable
    {
        public int Id { get; }

        internal IBehaviorTreeNode Node { get; }
        internal Port Input { get; private set; }
        internal Port Output { get; private set; }

        private readonly BehaviorTreeView _graph;

        public NodeView(BehaviorTreeView graph, IBehaviorTreeNode node)
            : base(Path.Combine(Utilities.GetCurrentDirectoryProjectRelativePath(), "NodeView.uxml"))
        {
            Node = node;
            _graph = graph;

            title = node.Name;
            if (title.EndsWith("Node")) title = title.Substring(0, title.Length - "Node".Length);

            Id = node.Id;

            style.left = node.Position.x;
            style.top = node.Position.y;

            CreateInputPort();
            CreateOutputPort();

            AddToClassList(node.BehaviorType.ToString().ToLower());
            AddToClassList(node.NodeType.Name);

            Bind();
        }

        public void Dispose()
        {
            Unbind();
            Node.Dispose();
        }

        void Bind()
        {
            Node.OnSelected += Select;
        }

        void Unbind()
        {
            Node.OnSelected -= Select;
        }

        private void CreateInputPort()
        {
            Input = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(NodeView));
            Input.portName = "";
            inputContainer.Add(Input);
        }

        private void CreateOutputPort()
        {
            switch (Node.BehaviorType)
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

            void CreateOutputPort(Port.Capacity capacity)
            {
                Output = InstantiatePort(Orientation.Vertical, Direction.Output, capacity, typeof(NodeView));
                Output.portName = "";
                outputContainer.Add(Output);
            }
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
            Node.IsSelected = true;
        }

        public override void OnUnselected()
        {
            base.OnUnselected();
            Node.IsSelected = false;
        }

        public void SyncPosition()
        {
            Node.Position = GetPosition().position;
        }

        private void Select()
        {
            Select(_graph, additive: false);
            _graph.FrameSelection();
        }
    }
}