using GraphExt;
using GraphExt.Editor;
using OneShot;

namespace EntitiesBT.Editor
{
    public class BehaviorTreeNodePropertyInstaller : IGraphInstaller
    {
        public void Install(Container container, TypeContainers typeContainers)
        {
            var presenterContainer = typeContainers.GetTypeContainer(typeof(NodeViewPresenter));

            presenterContainer.RegisterSingleton<ConvertToNodeData>(() =>
            {
                var graph = container.Resolve<ISerializableGraphBackend<EntitiesBT.BehaviorTreeNode, BehaviorTreeNodeComponent>>();
                return (in NodeId nodeId) => graph.NodeMap[nodeId].FindNodeProperties(graph.SerializedObjects[nodeId]);
            });

            presenterContainer.RegisterSingleton<FindPortData>(() =>
            {
                var graph = container.Resolve<ISerializableGraphBackend<EntitiesBT.BehaviorTreeNode, BehaviorTreeNodeComponent>>();
                return (in NodeId nodeId) => graph.NodeMap[nodeId].FindNodePorts(graph.SerializedObjects[nodeId]);
            });
        }
    }
}