using System;
using System.Collections.Generic;
using System.Linq;
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

        public Container Container { get; private set; }

        public void Install(Container rootContainer, TypeContainers typeContainers, GameObject prefabStageRoot)
        {
            Container = typeContainers.CreateTypeContainer(
                rootContainer,
                typeof(GraphRuntime<TNode>),
                typeof(GameObjectNodes<TNode, TComponent>)
            );

            Container.RegisterInstance(NodeViewFactory);

            RegisterGraph();
            RegisterFindCompatiblePort();
            RegisterNodeViewPresenter();
            RegisterEdgePresenter();
            RegisterElementMovedEventEmitter();
            RegisterSelection();

            void RegisterGraph()
            {
                var graphBackend = new GameObjectNodes<TNode, TComponent>(prefabStageRoot);
                rootContainer.RegisterInstance(graphBackend);
                rootContainer.RegisterSerializableGraphBackend(graphBackend);
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

            void RegisterEdgePresenter()
            {
                rootContainer.RegisterSingleton<IWindowSystem>(() => Container.Instantiate<EdgeRuntimeObserver<TNode>>());
            }

            void RegisterElementMovedEventEmitter()
            {
                rootContainer.RegisterSingleton<IWindowSystem>(() => Container.Instantiate<ElementMovedEventEmitter>());
            }

            void RegisterNodeViewPresenter()
            {
                Container.RegisterSingleton<ConvertToNodeData>(() => {
                    var graph = Container.Resolve<ISerializableGraphBackend<TNode, TComponent>>();
                    return (in NodeId nodeId) => graph.NodeMap[nodeId].FindNodeProperties(graph.SerializedObjects[nodeId]);
                });

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

                Container.Register(() => Container.Resolve<GraphRuntime<TNode>>().Nodes.Select(t => t.Item1));

                rootContainer.RegisterSingleton<IWindowSystem>(() => Container.Instantiate<NodeViewPresenter>());
            }

            void RegisterSelection()
            {
                rootContainer.RegisterSingleton<IWindowSystem>(() =>
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

                rootContainer.RegisterSingleton<IWindowSystem>(() =>
                {
                    var nodes = Container.Resolve<IReadOnlyDictionary<NodeId, Node>>();
                    var nodeObjects = Container.Resolve<IReadOnlyDictionary<NodeId, TComponent>>();
                    return new ActiveSelectedNodePresenter<TComponent>(nodes, nodeObjects, node =>
                    {
                        if (Selection.activeObject != node) Selection.activeObject = node;
                    });
                });
            }
        }
    }
}