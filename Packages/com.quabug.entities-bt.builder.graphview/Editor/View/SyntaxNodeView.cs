using System.IO;
using EntitiesBT.Core;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace EntitiesBT.Editor
{
    public sealed class SyntaxNodeView : Node, INodeView, ITickableElement
    {
        public ISyntaxTreeNode Node { get; }
        private BehaviorTreeView _graph;
        private readonly VisualElement _contentContainer;
        private readonly PropertyPortSystem _propertyPortSystem;

        public SyntaxNodeView(BehaviorTreeView graph, ISyntaxTreeNode node)
            : base(Path.Combine(Utilities.GetCurrentDirectoryProjectRelativePath(), "SyntaxNodeView.uxml"))
        {
            Node = node;
            _graph = graph;
            _contentContainer = this.Q<VisualElement>("contents");

            style.left = node.Position.x;
            style.top = node.Position.y;

            this.Q<VisualElement>("left-port").Add(CreateNodePort(Direction.Input));
            this.Q<VisualElement>("right-port").Add(CreateNodePort(Direction.Output));

            _propertyPortSystem = new PropertyPortSystem(_contentContainer);

            node.OnSelected += Select;

            Port CreateNodePort(Direction direction)
            {
                var port = InstantiatePort(Orientation.Horizontal, direction, Port.Capacity.Multi, node.VariantType);
                port.portName = "";
                port.AddToClassList("variant");
                return port;
            }
        }

        public void SyncPosition()
        {
            Node.Position = GetPosition().position;
        }

        public void Dispose()
        {
            Node.OnSelected -= Select;
            Node.Dispose();
        }

        public void Tick()
        {
            title = Node.Name;
            _propertyPortSystem.Refresh(Node.NodeObject);
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
    }
}