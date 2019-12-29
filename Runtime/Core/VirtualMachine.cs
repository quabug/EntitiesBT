using System;
using System.Collections.Generic;
using System.Data;

namespace EntitiesBT.Core
{
    using ResetFunc = Action<int, INodeBlob, IBlackboard>;
    using TickFunc = Func<int, INodeBlob, IBlackboard, NodeState>;
    
    public static class VirtualMachine
    {
        private static readonly Dictionary<int, ResetFunc> _resets = new Dictionary<int, ResetFunc>();
        private static readonly Dictionary<int, TickFunc> _ticks = new Dictionary<int, TickFunc>();

        public static void Register(int id, ResetFunc reset, TickFunc tick)
        {
            if (_resets.ContainsKey(id)) throw new DuplicateNameException($"Reset function {id} already registered");
            if (_ticks.ContainsKey(id)) throw new DuplicateNameException($"Tick function {id} already registered");

            _resets[id] = reset;
            _ticks[id] = tick;
        }
        
        public static NodeState Tick(INodeBlob blob, IBlackboard bb)
        {
            return Tick(0, blob, bb);
        }
        
        public static NodeState Tick(int index, INodeBlob blob, IBlackboard bb)
        {
            var typeId = blob.GetTypeId(index);
            var state = _ticks[typeId](index, blob, bb);
            // Debug.Log($"[BT] tick: {index}-{node.GetType().Name}-{state}");
            return state;
        }

        public static void Reset(int index, INodeBlob blob, IBlackboard bb)
        {
            var typeId = blob.GetTypeId(index);
            _resets[typeId](index, blob, bb);
            // Debug.Log($"[BT] tick: {index}-{node.GetType().Name}-{state}");
        }

        public static void Reset(int fromIndex, int count, INodeBlob blob, IBlackboard bb)
        {
            for (var i = fromIndex; i < fromIndex + count; i++)
                Reset(i, blob, bb);
        }

        public static void Reset(INodeBlob blob, IBlackboard bb)
        {
            var count = blob.GetEndIndex(0);
            Reset(0, count, blob, bb);
        }
    }
}