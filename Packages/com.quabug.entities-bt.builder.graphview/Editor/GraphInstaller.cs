using System;
using System.Collections.Generic;
using System.Linq;
using GraphExt;
using GraphExt.Editor;
using OneShot;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using GraphView = GraphExt.Editor.GraphView;

namespace EntitiesBT.Editor
{
    [Serializable]
    public class GraphInstaller
    {
        [SerializeReference, Nuwa.SerializeReferenceDrawer(Nullable = false, RenamePatter = @"\w*\.||")]
        public IGraphViewFactory GraphViewFactory = new DefaultGraphViewFactory();

        [SerializeReference, Nuwa.SerializeReferenceDrawer(Nullable = false, RenamePatter = @"\w*\.||")]
        public INodeViewFactory NodeViewFactory = new DefaultNodeViewFactory();

        [SerializeReference, Nuwa.SerializeReferenceDrawer(Nullable = false, RenamePatter = @"\w*\.||")]
        public IEdgeViewFactory EdgeViewFactory = new DefaultEdgeViewFactory();

        [SerializeReference, Nuwa.SerializeReferenceDrawer(Nullable = false, RenamePatter = @"\w*\.||")]
        public IPortViewFactory PortViewFactory = new DefaultPortViewFactory();

        public void Install(Container rootContainer, TypeContainers typeContainers, GameObject prefabStageRoot)
        {
            rootContainer.RegisterInstance<IGraphViewFactory>(GraphViewFactory);
            rootContainer.RegisterInstance<IEdgeViewFactory>(EdgeViewFactory);
            rootContainer.RegisterInstance<IPortViewFactory>(PortViewFactory);
            rootContainer.RegisterInstance<INodeViewFactory>(NodeViewFactory);

            rootContainer.RegisterBiDictionaryInstance(new BiDictionary<NodeId, Node>());
            rootContainer.RegisterBiDictionaryInstance(new BiDictionary<PortId, Port>());
            rootContainer.RegisterBiDictionaryInstance(new BiDictionary<EdgeId, Edge>());
            rootContainer.RegisterDictionaryInstance(new Dictionary<PortId, PortData>());
            rootContainer.Register<IEnumerable<EdgeId>>(() => rootContainer.Resolve<GraphRuntime<GraphNode>>().Edges);

            RegisterGraphView();
            RegisterGraph();
            RegisterNodePresenter();
            RegisterEdgePresenter();
            RegisterPortPresenter();
            RegisterElementMovedEventEmitter();
            RegisterSelection();

            void RegisterGraphView()
            {
                rootContainer.RegisterSingleton<GraphView>(() =>
                {
                    var factory = rootContainer.Resolve<IGraphViewFactory>();
                    var nodes = rootContainer.Resolve<GameObjectNodes<GraphNode, GraphNodeComponent>>();
                    var portIdDataMap = rootContainer.Resolve<IReadOnlyDictionary<PortId, PortData>>();
                    var portViewIdMap = rootContainer.Resolve<IReadOnlyDictionary<Port, PortId>>();
                    var graph = rootContainer.Resolve<GraphRuntime<GraphNode>>();

                    return factory.Create(EdgeFunctions.CreateFindCompatiblePortsFunc(portViewIdMap, IsEdgeCompatibleFunc));

                    bool IsEdgeCompatibleFunc(in PortId input, in PortId output)
                    {
                        var inputPort = portIdDataMap[input];
                        var outputPort = portIdDataMap[output];
                        var inputNode = nodes[input.NodeId];
                        var outputNode = nodes[output.NodeId];
                        return // single port could be handled by Unity Graph
                            (inputPort.Capacity == 1 || CountConnections(input) < inputPort.Capacity) &&
                            (outputPort.Capacity == 1 || CountConnections(output) < outputPort.Capacity) &&
                            graph.GetNodeByPort(input).IsPortCompatible(graph, input, output) &&
                            graph.GetNodeByPort(output).IsPortCompatible(graph, input, output) &&
                            inputNode.IsPortCompatible(nodes, input, output) &&
                            outputNode.IsPortCompatible(nodes, input, output)
                        ;
                    }

                    int CountConnections(PortId portId) => graph.Edges.Count(edge => edge.Contains(portId));
                });
                rootContainer.Register<GraphView, UnityEditor.Experimental.GraphView.GraphView>();
            }

            void RegisterGraph()
            {
                var graphBackend = new GraphGameObjectNodes(prefabStageRoot);
                rootContainer.RegisterInstance<GraphGameObjectNodes>(graphBackend);
                rootContainer.Register<GraphGameObjectNodes, GameObjectNodes<GraphNode, GraphNodeComponent>>();
                rootContainer.RegisterSerializableGraphBackend(graphBackend);
            }

            void RegisterEdgePresenter()
            {
                rootContainer.RegisterSingleton<EdgeConnectFunc>(() => EdgeFunctions.Connect(rootContainer.Resolve<GraphRuntime<GraphNode>>()));
                rootContainer.RegisterSingleton<EdgeDisconnectFunc>(() => EdgeFunctions.Disconnect(rootContainer.Resolve<GraphRuntime<GraphNode>>()));
                rootContainer.RegisterSingleton<IWindowSystem>(rootContainer.Instantiate<EdgeViewInitializer>);
                rootContainer.RegisterSingleton<IWindowSystem>(rootContainer.Instantiate<EdgeViewObserver>);
                rootContainer.RegisterSingleton<IWindowSystem>(rootContainer.Instantiate<EdgeRuntimeObserver<GraphNode>>);
            }

            void RegisterPortPresenter()
            {
                rootContainer.RegisterSingleton<FindPortData>(() =>
                {
                    var nodes = rootContainer.Resolve<ISerializableGraphBackend<GraphNode, GraphNodeComponent>>();
                    return (in NodeId nodeId) => nodes.NodeMap[nodeId].FindNodePorts(nodes.SerializedObjects[nodeId]);
                });
                rootContainer.RegisterSingleton<IWindowSystem>(rootContainer.Instantiate<DynamicPortsPresenter>);
            }

            void RegisterElementMovedEventEmitter()
            {
                rootContainer.RegisterSingleton<IWindowSystem>(rootContainer.Instantiate<ElementMovedEventEmitter>);
            }

            void RegisterNodePresenter()
            {
                rootContainer.RegisterSingleton<ConvertToNodeData>(() => {
                    var graph = rootContainer.Resolve<ISerializableGraphBackend<GraphNode, GraphNodeComponent>>();
                    return (in NodeId nodeId) => graph.NodeMap[nodeId].FindNodeProperties(graph.SerializedObjects[nodeId]);
                });

                rootContainer.RegisterSingleton<NodeViewPresenter.NodeAddedEvent>(() =>
                {
                    var graphRuntime = rootContainer.Resolve<GraphRuntime<GraphNode>>();
                    var added = new NodeViewPresenter.NodeAddedEvent();
                    graphRuntime.OnNodeAdded += (in NodeId id, GraphNode _) => added.Event?.Invoke(id);
                    return added;
                });

                rootContainer.RegisterSingleton<NodeViewPresenter.NodeDeletedEvent>(() =>
                {
                    var graphRuntime = rootContainer.Resolve<GraphRuntime<GraphNode>>();
                    var deleted = new NodeViewPresenter.NodeDeletedEvent();
                    graphRuntime.OnNodeWillDelete += (in NodeId id, GraphNode _) => deleted.Event?.Invoke(id);
                    return deleted;
                });

                rootContainer.Register<IEnumerable<NodeId>>(() => rootContainer.Resolve<GraphRuntime<GraphNode>>().Nodes.Select(t => t.Item1));
                rootContainer.RegisterSingleton<IWindowSystem>(rootContainer.Instantiate<NodeViewPresenter>);
            }

            void RegisterSelection()
            {
                rootContainer.RegisterSingleton<IWindowSystem>(() =>
                {
                    var graphView = rootContainer.Resolve<UnityEditor.Experimental.GraphView.GraphView>();
                    var nodeViews = rootContainer.Resolve<IReadOnlyBiDictionary<NodeId, Node>>();
                    var nodes = rootContainer.Resolve<IReadOnlyBiDictionary<NodeId, GraphNodeComponent>>();
                    return new SyncSelectionGraphElementPresenter(
                        graphView,
                        selectable => selectable is Node node ? nodes[nodeViews.Reverse[node]].gameObject : null,
                        obj =>
                        {
                            var nodeComponent = obj is GameObject node ? node.GetComponent<GraphNodeComponent>() : null;
                            return nodeComponent == null ? null : nodeViews[nodes.Reverse[nodeComponent]];
                        });
                });
            }
        }
    }
}