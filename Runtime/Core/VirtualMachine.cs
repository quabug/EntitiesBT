using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Unity.Entities;

namespace EntitiesBT.Core
{
    public static class VirtualMachine
    {
        public static NodeState Tick([NotNull] INodeBlob blob, [NotNull] IBlackboard bb)
        {
            return Tick(0, blob, bb);
        }
        
        public static NodeState Tick(int index, [NotNull] INodeBlob blob, [NotNull] IBlackboard bb)
        {
            var typeId = blob.GetTypeId(index);
            var ptr = blob.GetRuntimeDataPtr(index);
            var state = MetaNodeRegister.NODES[typeId].Tick.Invoke(ptr, index, blob, bb);
            blob.SetState(index, state);
            return state;
        }

        public static void Reset(int fromIndex, [NotNull] INodeBlob blob, [NotNull] IBlackboard bb, int count = 1)
        {
            blob.ResetStates(fromIndex, count);
            blob.ResetRuntimeData(fromIndex, count);
            for (var i = fromIndex; i < fromIndex + count; i++)
            {
                var typeId = blob.GetTypeId(i);
                var ptr = blob.GetRuntimeDataPtr(i);
                MetaNodeRegister.NODES[typeId].Reset.Invoke(ptr, i, blob, bb);
            }
        }

        public static void Reset([NotNull] INodeBlob blob, [NotNull] IBlackboard bb)
        {
            var count = blob.GetEndIndex(0);
            Reset(0, blob, bb, count);
        }

        [Pure]
        public static IEnumerable<ComponentType> GetAccessTypes(int index, [NotNull] INodeBlob blob)
        {
            var typeId = blob.GetTypeId(index);
            var ptr = blob.GetRuntimeDataPtr(index);
            var node = MetaNodeRegister.NODES[typeId];
            var runtimeTypes = node.RuntimeTypes.SelectMany(r => r.Func.Invoke(ptr + r.Offset));
            return node.StaticTypes.Concat(runtimeTypes);
        }
        
        [Pure]
        public static Type GetNodeType(int nodeId)
        {
            return MetaNodeRegister.NODES[nodeId].Type;
        }
    }
}