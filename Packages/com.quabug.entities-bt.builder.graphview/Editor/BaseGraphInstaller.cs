using System;
using System.Collections.Generic;
using System.Linq;
using GraphExt;
using GraphExt.Editor;
using OneShot;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using GraphView = GraphExt.Editor.GraphView;

namespace EntitiesBT.Editor
{
    [Serializable]
    public class BaseGraphInstaller
    {
        [SerializeReference, Nuwa.SerializeReferenceDrawer(Nullable = false, RenamePatter = @"\w*\.||")]
        public IGraphViewFactory GraphViewFactory = new DefaultGraphViewFactory();

        [SerializeReference, Nuwa.SerializeReferenceDrawer(Nullable = false, RenamePatter = @"\w*\.||")]
        public IEdgeViewFactory EdgeViewFactory = new DefaultEdgeViewFactory();

        [SerializeReference, Nuwa.SerializeReferenceDrawer(Nullable = false, RenamePatter = @"\w*\.||")]
        public IPortViewFactory PortViewFactory = new DefaultPortViewFactory();

        public Container Container { get; private set; }

        public void Install(Container rootContainer, TypeContainers typeContainers)
        {
            rootContainer.RegisterInstance(GraphViewFactory);
            rootContainer.RegisterInstance(EdgeViewFactory);
            rootContainer.RegisterInstance(PortViewFactory);

            Container = typeContainers.CreateTypeContainer(
                rootContainer,
                typeof(IGraphViewFactory),
                typeof(GraphView),
                typeof(UnityEditor.Experimental.GraphView.GraphView)
            );

            rootContainer.RegisterBiDictionaryInstance(new BiDictionary<NodeId, Node>());
            rootContainer.RegisterBiDictionaryInstance(new BiDictionary<PortId, Port>());
            rootContainer.RegisterBiDictionaryInstance(new BiDictionary<EdgeId, Edge>());
            rootContainer.RegisterDictionaryInstance(new Dictionary<PortId, PortData>());
            rootContainer.Register<IEnumerable<EdgeId>>(() =>
                rootContainer.Resolve<GraphRuntime<BehaviorTreeNode>>().Edges
                    .Concat(rootContainer.Resolve<GraphRuntime<VariantNode>>().Edges)
                    .Distinct()
            );

            Container.RegisterSingleton(() => GraphViewFactory.Create(FindCompatiblePorts));
            rootContainer.Register(Container.Resolve<GraphView>);
            rootContainer.Register<UnityEditor.Experimental.GraphView.GraphView>(Container.Resolve<GraphView>);

            Container.RegisterSingleton<EdgeDisconnectFunc>(CreateEdgeDisconnectFunc);
            Container.RegisterSingleton<EdgeConnectFunc>(CreateEdgeConnectFunc);
            rootContainer.RegisterSingleton<IWindowSystem>(() => Container.Instantiate<EdgeViewInitializer>());
            rootContainer.RegisterSingleton<IWindowSystem>(() => Container.Instantiate<EdgeViewObserver>());

            rootContainer.RegisterSingleton(FindPortDataFunc);
            rootContainer.RegisterSingleton<IWindowSystem>(() => Container.Instantiate<DynamicPortsPresenter>());

            FindPortData FindPortDataFunc()
            {
                var behaviorGraph = rootContainer.Resolve<ISerializableGraphBackend<BehaviorTreeNode, BehaviorTreeNodeComponent>>();
                var variantGraph = rootContainer.Resolve<ISerializableGraphBackend<VariantNode, VariantNodeComponent>>();
                return (in NodeId nodeId) =>
                {
                    if (behaviorGraph.NodeMap.TryGetValue(nodeId, out var behaviorNode))
                        return behaviorNode.FindNodePorts(behaviorGraph.SerializedObjects[nodeId]);
                    if (variantGraph.NodeMap.TryGetValue(nodeId, out var variantNode))
                        return variantNode.FindNodePorts(variantGraph.SerializedObjects[nodeId]);
                    return Enumerable.Empty<PortData>();
                };
            }

            IEnumerable<Port> FindCompatiblePorts(Port startPort)
            {
                var behaviorGraphContainer = typeContainers.GetTypeContainer(typeof(GraphRuntime<BehaviorTreeNode>));
                var behaviorGraphFunc = behaviorGraphContainer.Resolve<GraphView.FindCompatiblePorts>();

                var variantGraphContainer = typeContainers.GetTypeContainer(typeof(GraphRuntime<VariantNode>));
                var variantGraphFunc = variantGraphContainer.Resolve<GraphView.FindCompatiblePorts>();

                // HACK: assume that vertical port must be a tree port
                //       and horizontal port must be a variant port
                var isTreePort = startPort.orientation == Orientation.Vertical;
                return isTreePort ? behaviorGraphFunc(startPort) : variantGraphFunc(startPort);
            }

            EdgeConnectFunc CreateEdgeConnectFunc()
            {
                var behaviorGraph = rootContainer.Resolve<GraphRuntime<BehaviorTreeNode>>();
                var variantGraph = rootContainer.Resolve<GraphRuntime<VariantNode>>();
                return (in PortId input, in PortId output) =>
                {
                    var edge = new EdgeId(input, output);
                    if (!behaviorGraph.Edges.Contains(edge)) behaviorGraph.Connect(input: input, output: output);
                    if (!variantGraph.Edges.Contains(edge)) variantGraph.Connect(input: input, output: output);
                };
            }

            EdgeDisconnectFunc CreateEdgeDisconnectFunc()
            {
                var behaviorGraph = rootContainer.Resolve<GraphRuntime<BehaviorTreeNode>>();
                var variantGraph = rootContainer.Resolve<GraphRuntime<VariantNode>>();
                return (in PortId input, in PortId output) =>
                {
                    var edge = new EdgeId(input, output);
                    if (behaviorGraph.Edges.Contains(edge)) behaviorGraph.Disconnect(input: input, output: output);
                    if (variantGraph.Edges.Contains(edge)) variantGraph.Disconnect(input: input, output: output);
                };
            }
        }
    }
}