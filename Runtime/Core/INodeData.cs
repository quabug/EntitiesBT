using System.Collections.Generic;
using Unity.Entities;

namespace EntitiesBT.Core
{
    public interface INodeData
    {
        IEnumerable<ComponentType> AccessTypes(int index, INodeBlob blob);
        NodeState Tick(int index, INodeBlob blob, IBlackboard blackboard);
        void Reset(int index, INodeBlob blob, IBlackboard blackboard);
    }
}
