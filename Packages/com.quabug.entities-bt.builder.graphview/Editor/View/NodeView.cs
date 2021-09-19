using System;
using System.IO;
using EntitiesBT.Core;
using JetBrains.Annotations;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
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

        private Toggle ToggleActivation => this.Q<Toggle>("activation");
        private Label LabelTitle => this.Q<Label>("title-label");

        public NodeView(BehaviorTreeView graph, IBehaviorTreeNode node)
            : base(Path.Combine(Utilities.GetCurrentDirectoryProjectRelativePath(), "NodeView.uxml"))
        {
            Node = node;
            _graph = graph;

            Id = node.Id;

            style.left = node.Position.x;
            style.top = node.Position.y;

            CreateInputPort();
            CreateOutputPort();

            AddToClassList(node.BehaviorType.ToString().ToLower());
            AddToClassList(node.NodeType.Name);

            Bind(Node);
        }

        public void Dispose()
        {
            Unbind(Node);
            Node.Dispose();
        }

        private void Bind(IBehaviorTreeNode node)
        {
            node.OnSelected += Select;
            ToggleActivation.BindProperty(node.IsActive);
            SetName(node.Name);
            this.TrackPropertyValue(node.Name, SetName);
            this.TrackPropertyValue(node.IsActive, ResetActiveClass);

            void SetName(SerializedProperty nameProperty)
            {
                title = nameProperty.stringValue.TrimEnd("Node");
            }

            void ResetActiveClass(SerializedProperty isActive)
            {
                if (isActive.boolValue) RemoveFromClassList("disabled");
                else AddToClassList("disabled");
            }
        }

        void Unbind(IBehaviorTreeNode node)
        {
            this.Unbind();
            ToggleActivation.Unbind();
            node.OnSelected -= Select;
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
                    Create(Port.Capacity.Multi);
                    break;
                case BehaviorNodeType.Decorate:
                    Create(Port.Capacity.Single);
                    break;
                case BehaviorNodeType.Action:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            void Create(Port.Capacity capacity)
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
            if (!_graph.selection.Contains(this))
            {
                Select(_graph, additive: false);
                _graph.FrameSelection();
            }
        }
    }
}