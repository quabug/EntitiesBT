using System;
using System.Collections.Generic;
using System.Linq;
using GraphExt;
using GraphExt.Editor;
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
            rootContainer.RegisterInstance(GraphViewFactory).As<IGraphViewFactory>();
            rootContainer.RegisterInstance(EdgeViewFactory).As<IEdgeViewFactory>();
            rootContainer.RegisterInstance(PortViewFactory).As<IPortViewFactory>();
            rootContainer.RegisterInstance(NodeViewFactory).As<INodeViewFactory>();

            rootContainer.RegisterBiDictionaryInstance(new BiDictionary<NodeId, Node>());
            rootContainer.RegisterBiDictionaryInstance(new BiDictionary<PortId, Port>());
            rootContainer.RegisterBiDictionaryInstance(new BiDictionary<EdgeId, Edge>());
            rootContainer.RegisterDictionaryInstance(new Dictionary<PortId, PortData>());
            rootContainer.Register<IEnumerable<EdgeId>>((_, __) => rootContainer.Resolve<GraphRuntime<GraphNode>>().Edges).AsSelf();

            RegisterGraphView();
            RegisterGraph();
            RegisterNodePresenter();
            RegisterEdgePresenter();
            RegisterPortPresenter();
            RegisterElementMovedEventEmitter();
            RegisterSelection();

            void RegisterGraphView()
            {
                rootContainer.Register((_, __) =>
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
                }).Singleton().As<GraphView>().As<UnityEditor.Experimental.GraphView.GraphView>();
            }

            void RegisterGraph()
            {
                var graphBackend = new GraphGameObjectNodes(prefabStageRoot);
                rootContainer.RegisterInstance<GraphGameObjectNodes>(graphBackend).AsSelf().As<GameObjectNodes<GraphNode, GraphNodeComponent>>();
                rootContainer.RegisterSerializableGraphBackend(graphBackend);
            }

            void RegisterEdgePresenter()
            {
                rootContainer.Register<EdgeConnectFunc>((_, __) => EdgeFunctions.Connect(rootContainer.Resolve<GraphRuntime<GraphNode>>())).Singleton().AsSelf();
                rootContainer.Register<EdgeDisconnectFunc>((_, __) => EdgeFunctions.Disconnect(rootContainer.Resolve<GraphRuntime<GraphNode>>())).Singleton().AsSelf();
                rootContainer.Register<EdgeViewInitializer>().Singleton().As<IWindowSystem>();
                rootContainer.Register<EdgeViewObserver>().Singleton().As<IWindowSystem>();
                rootContainer.Register<EdgeRuntimeObserver<GraphNode>>().Singleton().As<IWindowSystem>();
            }

            void RegisterPortPresenter()
            {
                rootContainer.Register<FindPortData>((_, __) =>
                {
                    var nodes = rootContainer.Resolve<ISerializableGraphBackend<GraphNode, GraphNodeComponent>>();
                    return (in NodeId nodeId) => nodes.NodeMap[nodeId].FindNodePorts(nodes.SerializedObjects[nodeId]);
                }).AsSelf();
                rootContainer.Register<DynamicPortsPresenter>().Singleton().As<IWindowSystem>();
            }

            void RegisterElementMovedEventEmitter()
            {
                rootContainer.Register<ElementMovedEventEmitter>().Singleton().As<IWindowSystem>();
            }

            void RegisterNodePresenter()
            {
                rootContainer.Register<ConvertToNodeData>((_, __) => {
                    var graph = rootContainer.Resolve<ISerializableGraphBackend<GraphNode, GraphNodeComponent>>();
                    return (in NodeId nodeId) => graph.NodeMap[nodeId].FindNodeProperties(graph.SerializedObjects[nodeId]);
                }).Singleton().AsSelf();

                rootContainer.Register<NodeViewPresenter.NodeAddedEvent>((_, __) =>
                {
                    var graphRuntime = rootContainer.Resolve<GraphRuntime<GraphNode>>();
                    var added = new NodeViewPresenter.NodeAddedEvent();
                    graphRuntime.OnNodeAdded += (in NodeId id, GraphNode _) => added.Event?.Invoke(id);
                    return added;
                }).Singleton().AsSelf();

                rootContainer.Register<NodeViewPresenter.NodeDeletedEvent>((_, __) =>
                {
                    var graphRuntime = rootContainer.Resolve<GraphRuntime<GraphNode>>();
                    var deleted = new NodeViewPresenter.NodeDeletedEvent();
                    graphRuntime.OnNodeWillDelete += (in NodeId id, GraphNode _) => deleted.Event?.Invoke(id);
                    return deleted;
                }).Singleton().AsSelf();

                rootContainer.Register<IEnumerable<NodeId>>((_, __) => rootContainer.Resolve<GraphRuntime<GraphNode>>().Nodes.Select(t => t.Item1)).AsSelf();
                rootContainer.Register<NodeViewPresenter>().Singleton().As<IWindowSystem>();
            }

            void RegisterSelection()
            {
                var presenterContainer = typeContainers.CreateSystemContainer(rootContainer, typeof(SyncSelectionGraphElementPresenter));
                presenterContainer.Register<PrefabNodeSelectionConvertor<NodeId, Node, GraphNodeComponent>>()
                    .Singleton()
                    .As<SyncSelectionGraphElementPresenter.IConvertor>()
                ;
            }
        }
    }
}