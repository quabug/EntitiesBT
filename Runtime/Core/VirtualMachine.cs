using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace EntitiesBT.Core
{
    using ResetFunc = Action<int, INodeBlob, IBlackboard>;
    using TickFunc = Func<int, INodeBlob, IBlackboard, NodeState>;
    
    public static class VirtualMachine
    {
        private static readonly Dictionary<int, ResetFunc> _resets = new Dictionary<int, ResetFunc>();
        private static readonly Dictionary<int, TickFunc> _ticks = new Dictionary<int, TickFunc>();

        static VirtualMachine()
        {
            foreach (var type in AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes()))
            {
                var attribute = type.GetCustomAttribute<BehaviorNodeAttribute>();
                if (attribute == null) continue;
                var resetFunc = GetResetFunc(type.GetMethod(attribute.ResetFunc));
                var tickFunc = GetTickFunc(type.GetMethod(attribute.TickFunc));
                Register(attribute.Id, resetFunc, tickFunc);
            }

            ResetFunc GetResetFunc(MethodInfo methodInfo)
            {
                return methodInfo == null
                    ? (index, blob, bb) => {}
                    : (ResetFunc)methodInfo.CreateDelegate(typeof(ResetFunc))
                ;
            }

            TickFunc GetTickFunc(MethodInfo methodInfo)
            {
                return methodInfo == null
                    ? (index, blob, bb) => NodeState.Running
                    : (TickFunc)methodInfo.CreateDelegate(typeof(TickFunc))
                ;
            }

            void Register(int id, ResetFunc reset, TickFunc tick)
            {
                if (_resets.ContainsKey(id)) throw new DuplicateNameException($"Reset function {id} already registered");
                if (_ticks.ContainsKey(id)) throw new DuplicateNameException($"Tick function {id} already registered");

                _resets[id] = reset;
                _ticks[id] = tick;
            }
        }
        
        public static NodeState Tick(INodeBlob blob, IBlackboard bb)
        {
            return Tick(0, blob, bb);
        }
        
        public static NodeState Tick(int index, INodeBlob blob, IBlackboard bb)
        {
            var typeId = blob.GetTypeId(index);
            var state = _ticks[typeId](index, blob, bb);
            return state;
        }

        public static void Reset(int index, INodeBlob blob, IBlackboard bb)
        {
            var typeId = blob.GetTypeId(index);
            _resets[typeId](index, blob, bb);
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