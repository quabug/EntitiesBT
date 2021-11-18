using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EntitiesBT.Core;
using JetBrains.Annotations;
using Nuwa.Editor;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace EntitiesBT.Editor
{
    public sealed class BehaviorNodeView : Node, IDisposable, INodeView, ITickableElement
    {
        public int Id { get; }

        internal IBehaviorTreeNode Node { get; }
        internal Port Input { get; private set; }
        internal Port Output { get; private set; }

        private readonly BehaviorTreeView _graph;
        private readonly Toggle _toggleActivation;
        private readonly VisualElement _contentContainer;

        public BehaviorNodeView(BehaviorTreeView graph, IBehaviorTreeNode node)
            : base(Path.Combine(Utilities.GetCurrentDirectoryProjectRelativePath(), "BehaviorNodeView.uxml"))
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
        }

        public void Dispose()
        {
            Unbind(Node);
            Node.Dispose();
        }

        private readonly IDictionary<string /* property path */, NodePropertyView> _propertyViews = new Dictionary<string, NodePropertyView>();

        public void Tick()
        {
            title = Node.Name.TrimEnd("Node");
            _toggleActivation.SetValueWithoutNotify(Node.IsActive);
            if (Node.IsActive) RemoveFromClassList("disabled");
            else AddToClassList("disabled");

            var removedViews = new HashSet<string>(_propertyViews.Keys);
            foreach (var property in Node.NodeObject.FindGraphNodeVariantProperties())
            {
                if (removedViews.Contains(property.propertyPath))
                {
                    removedViews.Remove(property.propertyPath);
                }
                else
                {
                    var view = new NodePropertyView(property.GetManagedFullType());
                    _propertyViews.Add(property.propertyPath, view);
                    _contentContainer.Add(view);
                }
            }

            foreach (var removed in removedViews)
            {
                var view = _propertyViews[removed];
                _propertyViews.Remove(removed);
                _contentContainer.Remove(view);
            }
        }

        private void Bind(IBehaviorTreeNode node)
        {
            node.OnSelected += Select;
            _toggleActivation.RegisterValueChangedCallback(ActiveToggleChanged);
        }

        void Unbind(IBehaviorTreeNode node)
        {
            this.Unbind();
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