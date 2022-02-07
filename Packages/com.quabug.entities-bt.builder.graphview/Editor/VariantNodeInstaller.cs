using System.Collections.Generic;
using GraphExt;
using GraphExt.Editor;
using OneShot;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.Experimental.SceneManagement;
using GraphView = UnityEditor.Experimental.GraphView.GraphView;

namespace EntitiesBT.Editor
{
    public class VariantNodeInstaller : IGraphInstaller
    {
        public class VariantNodeViewPresenter : NodeViewPresenter
        {
            public VariantNodeViewPresenter(GraphView view, INodeViewFactory nodeViewFactory, IPortViewFactory portViewFactory, IEnumerable<NodeId> initializeNodesProvider, NodeAddedEvent onNodeAdded, NodeDeletedEvent onNodeDeleted, ConvertToNodeData nodeConvertor, FindPortData findPorts, IBiDictionary<NodeId, Node> currentNodeViews, IBiDictionary<PortId, Port> currentPortViews, IDictionary<PortId, PortData> currentPortDataMap) : base(view, nodeViewFactory, portViewFactory, initializeNodesProvider, onNodeAdded, onNodeDeleted, nodeConvertor, findPorts, currentNodeViews, currentPortViews, currentPortDataMap)
            {
            }
        }

        public void Install(Container container, TypeContainers typeContainers)
        {
            var prefabStageRoot = PrefabStageUtility.GetCurrentPrefabStage().prefabContentsRoot;
            var graphBackend = new GameObjectNodes<VariantNode, VariantNodeComponent>(prefabStageRoot);
            container.RegisterInstance(graphBackend);
            container.RegisterSerializableGraphBackend(graphBackend);

            RegisterNodeViewPresenter(container, typeContainers);
            // RegisterEdgeViewPresenter(container, typeContainers);
        }

        void RegisterNodeViewPresenter(Container container, TypeContainers typeContainers)
        {
            var presenterContainer = typeContainers.CreateSystemContainer(container, typeof(VariantNodeViewPresenter));

            presenterContainer.RegisterSingleton<ConvertToNodeData>(() =>
            {
                var nodes = container.Resolve<IReadOnlyDictionary<NodeId, VariantNodeComponent>>();
                var nodeObjects = container.Resolve<IReadOnlyDictionary<NodeId, SerializedObject>>();
                return (in NodeId nodeId) => nodes[nodeId].FindNodeProperties(nodeObjects[nodeId]);
            });

            presenterContainer.RegisterSingleton<FindPortData>(() =>
            {
                var nodes = container.Resolve<IReadOnlyDictionary<NodeId, VariantNodeComponent>>();
                var nodeObjects = container.Resolve<IReadOnlyDictionary<NodeId, SerializedObject>>();
                return (in NodeId nodeId) => nodes[nodeId].FindNodePorts(nodeObjects[nodeId]);
            });

            // TODO: RX?
            presenterContainer.RegisterSingleton(() =>
            {
                var graphRuntime = presenterContainer.Resolve<GraphRuntime<VariantNode>>();
                var added = new NodeViewPresenter.NodeAddedEvent();
                graphRuntime.OnNodeAdded += (in NodeId id, VariantNode _) => added.Event?.Invoke(id);
                return added;
            });

            presenterContainer.RegisterSingleton(() =>
            {
                var graphRuntime = presenterContainer.Resolve<GraphRuntime<VariantNode>>();
                var deleted = new NodeViewPresenter.NodeDeletedEvent();
                graphRuntime.OnNodeWillDelete += (in NodeId id, VariantNode _) => deleted.Event?.Invoke(id);
                return deleted;
            });
        }

        void RegisterEdgeViewPresenter(Container container, TypeContainers typeContainers)
        {
            var presenterContainer = typeContainers.CreateSystemContainer(container, typeof(EdgeViewInitializer), typeof(EdgeViewObserver));
            presenterContainer.RegisterSingleton(() => EdgeFunctions.Connect(presenterContainer.Resolve<GraphRuntime<VariantNode>>()));
            presenterContainer.RegisterSingleton(() => EdgeFunctions.Disconnect(presenterContainer.Resolve<GraphRuntime<VariantNode>>()));
        }
    }
}