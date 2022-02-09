using System.Collections.Generic;
using GraphExt;

namespace EntitiesBT
{
    public interface IGraphNodeComponent
    {
#if UNITY_EDITOR
        NodeData FindNodeProperties(UnityEditor.SerializedObject nodeObject);
        IEnumerable<PortData> FindNodePorts(UnityEditor.SerializedObject nodeObject);
#endif
    }
}