using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EntitiesBT.Core;
using JetBrains.Annotations;
using Shtif;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace EntitiesBT.Editor
{
    public class BehaviorTreeView : GraphView
    {
        public new class UxmlFactory : UxmlFactory<BehaviorTreeView, UxmlTraits> {}

        private IBehaviorTreeGraph _graph;

        public BehaviorTreeView()
        {
            Insert(0, new GridBackground { name = "grid" });

            var miniMap = new MiniMap();
            Add(miniMap);
            // NOTE: not working... have to set `graphView` on `CreateGUI` of `BehaviorTreeEditor`
            miniMap.graphView = this;
            miniMap.windowed = true;
            miniMap.name = "minimap";

            this.AddManipulator(new ContentZoomer());
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
        }

        public void Reset([CanBeNull] IBehaviorTreeGraph graph)
        {
            graphViewChanged -= OnGraphChanged;

            _graph?.Dispose();
            _graph = graph;
            DeleteElements(graphElements.ToList());

            if (_graph != null)
            {
                foreach (var node in _graph.BehaviorTreeRootNodes) CreateBehaviorNode(node);
                foreach (var node in _graph.SyntaxTreeRootNodes) CreateSyntaxNode(node);
            }

            graphViewChanged += OnGraphChanged;

            GraphViewChange OnGraphChanged(GraphViewChange @event)
            {
                if (@event.elementsToRemove != null)
                {
                    foreach (var edge in @event.elementsToRemove.OfType<Edge>()) OnEdgeDeleted(edge);
                    foreach (var node in @event.elementsToRemove.OfType<INodeView>().ToArray())
                    {
                        node.Dispose();
                        edges.ForEach(edge =>
                        {
                            if (edge.input.node == node || edge.output.node == node)
                                @event.elementsToRemove.Add(edge);
                        });
                    }
                }

                if (@event.edgesToCreate != null)
                {
                    foreach (var edge in @event.edgesToCreate) OnEdgeCreated(edge, ref @event);
                }

                if (@event.movedElements != null)
                {
                    foreach (var node in @event.movedElements.OfType<INodeView>()) node.SyncPosition();
                }

                return @event;
            }

            void OnEdgeCreated(Edge edge, ref GraphViewChange @event)
            {
                edge.showInMiniMap = true;
                if (edge.input.node is BehaviorNodeView inputNode && edge.output.node is BehaviorNodeView outputNode)
                {
                    outputNode.ConnectTo(inputNode);
                }
                else
                {
                    var (syntaxNodeView, nodePropertyView) = FindVariantEdgeViews(edge);
                    if (syntaxNodeView != null && nodePropertyView != null)
                    {
                        nodePropertyView.ConnectTo(syntaxNodeView);

                        // disconnect edges on the same ports of NodePropertyView
                        edges.ForEach(e =>
                        {
                            if (e != edge && nodePropertyView.IsConnected(e))
                            {
                                e.input.Disconnect(e);
                                e.output.Disconnect(e);
                                RemoveElement(e);
                            }
                        });
                        // NOTE: not working since remove events are not handled in following process of `Port`
                        // var removedEdges = edges.ToList().Where(e => e != edge && nodePropertyView.IsConnected(e));
                        // @event.elementsToRemove ??= new List<GraphElement>();
                        // @event.elementsToRemove.AddRange(removedEdges);
                    }
                }
            }

            void OnEdgeDeleted(Edge edge)
            {
                if (edge.input.node is BehaviorNodeView inputNode && edge.output.node is BehaviorNodeView outputNode)
                {
                    inputNode.DisconnectFrom(outputNode);
                }
                else
                {
                    var (syntaxNodeView, nodePropertyView) = FindVariantEdgeViews(edge);
                    if (syntaxNodeView != null && nodePropertyView != null) nodePropertyView.DisconnectFrom(syntaxNodeView);
                }
            }

            (SyntaxNodeView, NodePropertyView) FindVariantEdgeViews(Edge edge)
            {
                var syntaxNodeView = edge.input.node as SyntaxNodeView;
                var propertyContainer = edge.output.node as INodePropertyContainer;
                var propertyView = propertyContainer.FindByPort(edge.output);
                if (syntaxNodeView == null || propertyContainer == null)
                {
                    syntaxNodeView = edge.output.node as SyntaxNodeView;
                    propertyContainer = edge.input.node as INodePropertyContainer;
                    propertyView = propertyContainer.FindByPort(edge.input);
                }
                return (syntaxNodeView, propertyView);
            }
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            var compatiblePorts = new List<Port>();
            ports.ForEach(port =>
            {
                if (port.direction != startPort.direction && port.node != startPort.node && port.portType == startPort.portType)
                    compatiblePorts.Add(port);
            });
            return compatiblePorts;
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            evt.StopPropagation();
            var menuPosition = viewTransform.matrix.inverse.MultiplyPoint(evt.localMousePosition);
            var context = new GenericMenu();

            FillSelectionActions();
            FillBehaviorNodes();
            context.AddSeparator("");
            FillVariantNodes();

            var popup = GenericMenuPopup.Get(context, "");
            popup.showSearch = true;
            popup.showTooltip = false;
            popup.resizeToContent = true;
            popup.Show(evt.mousePosition);

            void FillSelectionActions()
            {
                if (selection != null && selection.Any())
                {
                    context.AddItem(new GUIContent("Delete"), false, () => DeleteSelection());
                    context.AddSeparator("");
                }
            }

            void FillBehaviorNodes()
            {
                var types = TypeCache.GetTypesWithAttribute<BehaviorNodeAttribute>();
                foreach (var (type, attribute) in (
                    from type in types
                    from attribute in type.GetCustomAttributes<BehaviorNodeAttribute>()
                    where !attribute.Ignore
                    select (type, attribute)
                ).OrderBy(t => t.type.Name))
                {
                    var path = $"{attribute.Type}/{type.Name}";
                    context.AddItem(new GUIContent(path), false, Action);

                    void Action()
                    {
                        var node = _graph.AddBehaviorNode(type, menuPosition);
                        CreateBehaviorNode(node);
                    }
                }
            }

            void FillVariantNodes()
            {
                var variantNodeTypes = TypeCache.GetTypesDerivedFrom<VariantNode>();
                foreach (var variant in
                    from variant in variantNodeTypes
                    orderby variant.Name
                    select variant
                )
                {
                    var path = $"Variant/{variant.Name}";
                    context.AddItem(new GUIContent(path), false, Action);

                    void Action()
                    {
                        var variantNode = _graph.AddSyntaxNode(variant, menuPosition);
                        var view = new SyntaxNodeView(this, variantNode);
                        AddElement(view);
                    }
                }
            }
        }

        // TODO: optimize?
        private BehaviorNodeView CreateBehaviorNode(IBehaviorTreeNode node)
        {
            var nodeView = new BehaviorNodeView(this, node);
            AddElement(nodeView);
            foreach (var child in node.Children)
            {
                var childView = CreateBehaviorNode(child);
                var edge = nodeView.Output.ConnectTo(childView.Input);
                AddElement(edge);
            }
            return nodeView;
        }

        private SyntaxNodeView CreateSyntaxNode(ISyntaxTreeNode node)
        {
            var nodeView = new SyntaxNodeView(this, node);
            AddElement(nodeView);
            return nodeView;
        }
    }
}