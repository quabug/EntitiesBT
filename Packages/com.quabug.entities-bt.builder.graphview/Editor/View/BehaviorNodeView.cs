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
    public sealed class BehaviorNodeView : Node, IDisposable, INodeView
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

        private void Bind(IBehaviorTreeNode node)
        {
            node.OnSelected += Select;
            _toggleActivation.BindProperty(node.IsActive);
            SetName(node.Name);
            this.TrackPropertyValue(node.Name, SetName);
            this.TrackPropertyValue(node.IsActive, ResetActiveClass);

            var nodeProperty = node.NodeObject.GetIterator();
            nodeProperty.NextVisible(true);
            do
            {
                var propertyElements = nodeProperty.CreateNodeProperty();
                foreach (var element in propertyElements) _contentContainer.Add(element);
            } while (nodeProperty.NextVisible(false));

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
            _toggleActivation.Unbind();
            node.OnSelected -= Select;
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