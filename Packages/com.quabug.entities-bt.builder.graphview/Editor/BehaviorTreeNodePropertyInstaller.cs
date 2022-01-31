using System.Collections.Generic;
using GraphExt;
using GraphExt.Editor;
using OneShot;
using UnityEditor;

namespace EntitiesBT.Editor
{
    public class BehaviorTreeNodePropertyInstaller : IGraphInstaller
    {
        public void Install(Container container, TypeContainers typeContainers)
        {
            var presenterContainer = typeContainers.GetTypeContainer(typeof(NodeViewPresenter));

            presenterContainer.RegisterSingleton<ConvertToNodeData>(() =>
            {
                var nodes = container.Resolve<IReadOnlyDictionary<NodeId, GraphNodeComponent>>();
                var nodeObjects = container.Resolve<IReadOnlyDictionary<NodeId, SerializedObject>>();
                return (in NodeId nodeId) => nodes[nodeId].CreateNodeProperties(nodeObjects[nodeId]);
            });

            presenterContainer.RegisterSingleton<FindPortData>(() =>
            {
                var nodes = container.Resolve<IReadOnlyDictionary<NodeId, GraphNodeComponent>>();
                var nodeObjects = container.Resolve<IReadOnlyDictionary<NodeId, SerializedObject>>();
                return (in NodeId nodeId) => nodes[nodeId].FindNodePorts(nodeObjects[nodeId]);
            });

            // HACK: expose `FindPortData` to root container for other presenters
            container.Register(() => presenterContainer.Resolve<FindPortData>());
        }


    }
}