using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Unity.Entities;

namespace EntitiesBT.Core
{
    public static class VirtualMachine
    {
        public static NodeState Tick<TNodeBlob, TBlackboard>(ref TNodeBlob blob, ref TBlackboard bb)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
            return Tick(0, ref blob, ref bb);
        }
        
        public static NodeState Tick<TNodeBlob, TBlackboard>(int index, ref TNodeBlob blob, ref TBlackboard bb)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
            var typeId = blob.GetTypeId(index);
            var ptr = blob.GetRuntimeDataPtr(index);
            var state = MetaNodeRegister<TNodeBlob, TBlackboard>.NODES[typeId].Tick.Invoke(ptr, index, ref blob, ref bb);
            blob.SetState(index, state);
            return state;
        }

        public static void Reset<TNodeBlob, TBlackboard>(int fromIndex, ref TNodeBlob blob, ref TBlackboard bb, int count = 1)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
            blob.ResetStates(fromIndex, count);
            blob.ResetRuntimeData(fromIndex, count);
            for (var i = fromIndex; i < fromIndex + count; i++)
            {
                var typeId = blob.GetTypeId(i);
                var ptr = blob.GetRuntimeDataPtr(i);
                MetaNodeRegister<TNodeBlob, TBlackboard>.NODES[typeId].Reset.Invoke(ptr, i, ref blob, ref bb);
            }
        }

        public static void Reset<TNodeBlob, TBlackboard>(ref TNodeBlob blob, ref TBlackboard bb)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
            var count = blob.GetEndIndex(0);
            Reset(0, ref blob, ref bb, count);
        }

        [Pure]
        public static IEnumerable<ComponentType> GetAccessTypes<TNodeBlob>(int index, ref TNodeBlob blob)
            where TNodeBlob : struct, INodeBlob
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