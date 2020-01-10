using System.Collections.Generic;
using System.Linq;
using EntitiesBT.Entities;
using Unity.Collections;
using Unity.Entities;

namespace EntitiesBT.Core
{
    public interface INodeDataBuilder
    {
        BehaviorNodeType NodeType { get; }
        int NodeId { get; }
        int Size { get; }
        unsafe void Build(void* dataPtr);
        // IEnumerable<INodeDataBuilder> Chilren { get; }
    }

    public static class NodeDataExtensions
    {
        
    }
}
