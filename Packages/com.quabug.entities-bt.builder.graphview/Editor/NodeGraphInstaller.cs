using System;
using System.Collections.Generic;
using GraphExt;
using GraphExt.Editor;
using OneShot;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace EntitiesBT.Editor
{
    [Serializable]
    public class NodeGraphInstaller<TNode, TComponent>
        where TNode : INode<GraphRuntime<TNode>>
        where TComponent : MonoBehaviour, INodeComponent<TNode, TComponent>, IGraphNodeComponent
    {
        [SerializeReference, Nuwa.SerializeReferenceDrawer(Nullable = false, RenamePatter = @"\w*\.||")]
        public INodeViewFactory NodeViewFactory = new DefaultNodeViewFactory();

        [SerializeReference, Nuwa.SerializeReferenceDrawer(Nullable = false, RenamePatter = @"\w*\.||")]
        public IPortViewFactory PortViewFactory = new DefaultPortViewFactory();

        [SerializeReference, Nuwa.SerializeReferenceDrawer(Nullable = false, RenamePatter = @"\w*\.||")]
        public IEdgeViewFactory EdgeViewFactory = new DefaultEdgeViewFactory();

        [SerializeReference, SerializeReferenceDrawer(Nullable = false, RenamePatter = @"\w*\.||")]
        public IMenuEntryInstaller[] MenuEntries;

        public Container Container { get; private set; }

        public void Install(Container container, TypeContainers typeContainers, GameObject prefabStageRoot)
        {
            Container = typeContainers.CreateTypeContainer(
                container,
                typeof(GraphRuntime<TNode>),
                typeof(GameObjectNodes<TNode, TComponent>)
            );

            Container.RegisterInstance(NodeViewFactory);
            Container.RegisterInstance(PortViewFactory);
            Container.RegisterInstance(EdgeViewFactory);

            Container.RegisterBiDictionaryInstance(new BiDictionary<NodeId, Node>());
            Container.RegisterBiDictionaryInstance(new BiDictionary<PortId, Port>());
            Container.RegisterBiDictionaryInstance(new BiDictionary<EdgeId, Edge>());
            Container.RegisterDictionaryInstance(new Dictionary<PortId, PortData>());

            RegisterGraph();
            RegisterFindCompatiblePort();
            RegisterNodeViewPresenter();
            RegisterEdgeViewPresenter();
            RegisterElementMovedEventEmitter();
            RegisterSelection();
            RegisterMenuEntries();

            void RegisterGraph()
            {
                var graphBackend = new GameObjectNodes<TNode, TComponent>(prefabStageRoot);
                Container.RegisterInstance(graphBackend);
                Container.RegisterSerializableGraphBackend(graphBackend);
                Container.Register<Func<NodeId, INodeComponent>>(() =>
                {
                    var nodes = Container.Resolve<IReadOnlyDictionary<NodeId, TComponent>>();
                    return id => nodes[id];
                });
            }

            void RegisterFindCompatiblePort()
            {
                Func<GraphRuntime<TNode>, IReadOnlyDictionary<PortId, PortData>, IsEdgeCompatibleFunc>
                    isCompatible = EdgeFunctions.CreateIsCompatibleFunc;
                Container.RegisterSingleton<IsEdgeCompatibleFunc>(() =>
                {
                    var isCompatibleFunc = Container.Call<IsEdgeCompatibleFunc>(isCompatible);
                    var graph = Container.Resolve<GameObjectNodes<TNode, TComponent>>();
                    return (in PortId input, in PortId output) => isCompatibleFunc(input: input, output: output) &&
                                                                  graph.IsPortCompatible(input: input, output: output)
                    ;
                });

                Func<IReadOnlyDictionary<Port, PortId>, IsEdgeCompatibleFunc, GraphExt.Editor.GraphView.FindCompatiblePorts>
                    findCompatible = EdgeFunctions.CreateFindCompatiblePortsFunc;
                Container.RegisterSingleton(() => Container.Call<GraphExt.Editor.GraphView.FindCompatiblePorts>(findCompatible));
            }

            void RegisterEdgeViewPresenter()
            {
                Container.RegisterSingleton(() => EdgeFunctions.Connect(Container.Resolve<GraphRuntime<TNode>>()));
                Container.RegisterSingleton(() => EdgeFunctions.Disconnect(Container.Resolve<GraphRuntime<TNode>>()));
                container.RegisterSingleton<IWindowSystem>(() => Container.Instantiate<EdgeViewInitializer>());
                container.RegisterSingleton<IWindowSystem>(() => Container.Instantiate<EdgeViewObserver>());
                container.RegisterSingleton<IWindowSystem>(() => Container.Instantiate<EdgeRuntimeObserver<TNode>>());
            }

            void RegisterElementMovedEventEmitter()
            {
                container.RegisterSingleton<IWindowSystem>(() => Container.Instantiate<ElementMovedEventEmitter>());
            }

            void RegisterNodeViewPresenter()
            {
                Container.RegisterSingleton<ConvertToNodeData>(() => {
                    var graph = Container.Resolve<ISerializableGraphBackend<TNode, TComponent>>();
                    return (in NodeId nodeId) => graph.NodeMap[nodeId].FindNodeProperties(graph.SerializedObjects[nodeId]);
                });

                Container.RegisterSingleton(FindPortDataFunc);

                Container.RegisterSingleton(() =>
                {
                    var graphRuntime = Container.Resolve<GraphRuntime<TNode>>();
                    var added = new NodeViewPresenter.NodeAddedEvent();
                    graphRuntime.OnNodeAdded += (in NodeId id, TNode _) => added.Event?.Invoke(id);
                    return added;
                });

                Container.RegisterSingleton(() =>
                {
                    var graphRuntime = Container.Resolve<GraphRuntime<TNode>>();
                    var deleted = new NodeViewPresenter.NodeDeletedEvent();
                    graphRuntime.OnNodeWillDelete += (in NodeId id, TNode _) => deleted.Event?.Invoke(id);
                    return deleted;
                });

                container.RegisterSingleton<IWindowSystem>(() => Container.Instantiate<NodeViewPresenter>());
                container.RegisterSingleton<IWindowSystem>(() => Container.Instantiate<DynamicPortsPresenter>());

                FindPortData FindPortDataFunc()
                {
                    var graph = Container.Resolve<ISerializableGraphBackend<TNode, TComponent>>();
                    return (in NodeId nodeId) => graph.NodeMap[nodeId].FindNodePorts(graph.SerializedObjects[nodeId]);
                }
            }

            void RegisterSelection()
            {
                container.RegisterSingleton<IWindowSystem>(() =>
                {
                    var graphView = Container.Resolve<UnityEditor.Experimental.GraphView.GraphView>();
                    var nodeViews = Container.Resolve<IReadOnlyDictionary<NodeId, Node>>();
                    var nodes = Container.Resolve<IReadOnlyDictionary<TComponent, NodeId>>();
                    return new FocusActiveNodePresenter<TComponent>(
                        graphView,
                        node => nodeViews[nodes[node]],
                        () => Selection.activeObject as TComponent
                    );
                });

                container.RegisterSingleton<IWindowSystem>(() =>
                {
                    var nodes = Container.Resolve<IReadOnlyDictionary<NodeId, Node>>();
                    var nodeObjects = Container.Resolve<IReadOnlyDictionary<NodeId, TComponent>>();
                    return new ActiveSelectedNodePresenter<TComponent>(nodes, nodeObjects, node =>
                    {
                        if (Selection.activeObject != node) Selection.activeObject = node;
                    });
                });
            }

            void RegisterMenuEntries()
            {
                foreach (var entry in MenuEntries) entry.Install(Container);
            }
        }
    }
}