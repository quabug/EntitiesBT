using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace EntitiesBT.Editor
{
    public sealed class SyntaxNodeView : Node, INodeView, ITickableElement, IConnectableVariantViewContainer, IVariantPortContainer
    {
        private ISyntaxTreeNode _node;
        private BehaviorTreeView _graph;
        private readonly VisualElement _contentContainer;
        private readonly GraphNodeVariantPortSystem _graphNodeVariantPortSystem;

        public IReadOnlyList<Port> Ports { get; }
        public int Id { get; }

        public SyntaxNodeView(BehaviorTreeView graph, ISyntaxTreeNode node)
            : base(Path.Combine(Core.Utilities.GetCurrentDirectoryProjectRelativePath(), "SyntaxNodeView.uxml"))
        {
            _node = node;
            _graph = graph;
            _contentContainer = this.Q<VisualElement>("contents");

            Id = node.Id;

            style.left = node.Position.x;
            style.top = node.Position.y;

            Ports = new[]
            {
                Utilities.CreateVariantPort(Direction.Input, Port.Capacity.Multi, node.VariantType),
                Utilities.CreateVariantPort(Direction.Output, Port.Capacity.Multi, node.VariantType)
            };
            inputContainer.Add(Ports[0]);
            outputContainer.Add(Ports[1]);

            _graphNodeVariantPortSystem = new GraphNodeVariantPortSystem(_contentContainer, node);

            node.OnSelected += Select;
        }

        public void SyncPosition()
        {
            _node.Position = GetPosition().position;
        }

        public void Connect(GraphNodeVariant.Any variant, int variantPortIndex, Port syntaxNodePort)
        {
            var syntaxNodePortIndex = Ports.IndexOf(syntaxNodePort);
            _node.Connect(variant, variantPortIndex: variantPortIndex, syntaxNodePortIndex: syntaxNodePortIndex);
        }

        public void Disconnect(GraphNodeVariant.Any variant)
        {
            _node.Disconnect(variant);
        }

        public void Dispose()
        {
            _node.OnSelected -= Select;
            _node.Dispose();
        }

        public void Tick()
        {
            title = _node.Name;
            _graphNodeVariantPortSystem.Refresh();
            var variantType = _node.VariantType;
            foreach (var port in Ports.Where(p => p.portType != variantType))
            {
                port.RemoveFromClassList(Utilities.VariantAccessMode(port.portType));
                port.portType = variantType;
                port.AddToClassList(Utilities.VariantAccessMode(port.portType));
            }
        }

        public override void OnSelected()
        {
            base.OnSelected();
            _node.IsSelected = true;
        }

        public override void OnUnselected()
        {
            base.OnUnselected();
            _node.IsSelected = false;
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