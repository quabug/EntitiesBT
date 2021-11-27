using System;
using System.Collections.Generic;
using System.IO;
using EntitiesBT.Core;
using JetBrains.Annotations;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace EntitiesBT.Editor
{
    public sealed class BehaviorNodeView : Node, INodeView, ITickableElement, IConnectableVariantViewContainer
    {
        public int Id { get; }

        internal IBehaviorTreeNode Node { get; }
        internal Port Input { get; private set; }
        internal Port Output { get; private set; }

        private readonly BehaviorTreeView _graph;
        private readonly Toggle _toggleActivation;
        private readonly VisualElement _contentContainer;
        private readonly GraphNodeVariantPortSystem _graphNodeVariantPortSystem;

        public BehaviorNodeView(BehaviorTreeView graph, IBehaviorTreeNode node)
            : base(Path.Combine(Core.Utilities.GetCurrentDirectoryProjectRelativePath(), "BehaviorNodeView.uxml"))
        {
            Node = node;
            _graph = graph;

            Id = node.Id;

            style.left = node.Position.x;
            style.top = node.Position.y;

            _contentContainer = this.Q<VisualElement>("contents");
            _toggleActivation = this.Q<Toggle>("activation");

            CreateInputPort();
            CreateOutputPort();

            AddToClassList(node.BehaviorType.ToString().ToLower());
            AddToClassList(node.NodeType.Name);

            Bind(Node);

            _graphNodeVariantPortSystem = new GraphNodeVariantPortSystem(_contentContainer, node);
        }

        public void Dispose()
        {
            Unbind(Node);
            Node.Dispose();
        }

        public void Tick()
        {
            title = Node.Name.TrimEnd("Node");
            _toggleActivation.SetValueWithoutNotify(Node.IsActive);
            if (Node.IsActive) RemoveFromClassList("disabled");
            else AddToClassList("disabled");
            _graphNodeVariantPortSystem.Refresh();
        }

        private void Bind(IBehaviorTreeNode node)
        {
            node.OnSelected += Select;
            _toggleActivation.RegisterValueChangedCallback(ActiveToggleChanged);
        }

        void Unbind(IBehaviorTreeNode node)
        {
            _toggleActivation.UnregisterValueChangedCallback(ActiveToggleChanged);
            node.OnSelected -= Select;
        }

        void ActiveToggleChanged(ChangeEvent<bool> value)
        {
            if (value.newValue != Node.IsActive)
                Node.IsActive = value.newValue;
        }

        private void CreateInputPort()
        {
            Input = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(BehaviorNodeView));
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
                Output = InstantiatePort(Orientation.Vertical, Direction.Output, capacity, typeof(BehaviorNodeView));
                Output.portName = "";
                outputContainer.Add(Output);
            }
        }

        public void ConnectTo([NotNull] BehaviorNodeView child)
        {
            child.Node.SetParent(Node);
        }

        public void DisconnectFrom([NotNull] BehaviorNodeView parent)
        {
            Node.SetParent(null);
        }

        public void SyncPosition()
        {
            Node.Position = GetPosition().position;
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

        private void Select()
        {
            if (!_graph.selection.Contains(this))
            {
                Select(_graph, additive: false);
                _graph.FrameSelection();
            }
        }

        public ConnectableVariantView FindByPort(Port port) => _graphNodeVariantPortSystem.Find(port);
        public IEnumerable<ConnectableVariantView> Views => _graphNodeVariantPortSystem.Views;
    }
}