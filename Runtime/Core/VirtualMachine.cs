using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using Unity.Entities;

namespace EntitiesBT.Core
{
    public static class VirtualMachine
    {
        public delegate void ResetFunc(int nodeIndex, INodeBlob blob, IBlackboard bb);
        public delegate NodeState TickFunc(int nodeIndex, INodeBlob blob, IBlackboard bb);
        
        private static readonly Dictionary<int, ResetFunc> _RESETS = new Dictionary<int, ResetFunc>();
        private static readonly Dictionary<int, TickFunc> _TICKS = new Dictionary<int, TickFunc>();
        private static readonly Dictionary<int, ComponentType[]> _ACCESS_TYPES = new Dictionary<int, ComponentType[]>();

        public static ComponentType[] GetAccessTypes(int nodeId) => _ACCESS_TYPES[nodeId];

        static VirtualMachine()
        {
            foreach (var type in AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes()))
            {
                var attribute = type.GetCustomAttribute<BehaviorNodeAttribute>();
                if (attribute == null) continue;
                var resetFunc = GetResetFunc(type.GetMethod(attribute.ResetFunc));
                var tickFunc = GetTickFunc(type.GetMethod(attribute.TickFunc));
                var accessTypes = GetAccessTypes(type.GetField(attribute.TypesField));
                Register(attribute.Id, resetFunc, tickFunc, accessTypes);
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
            
            ComponentType[] GetAccessTypes(FieldInfo fieldInfo)
            {
                return fieldInfo == null
                    ? new ComponentType[0]
                    : (ComponentType[])fieldInfo.GetValue(null)
                ;
            }

            void Register(int id, ResetFunc reset, TickFunc tick, ComponentType[] types)
            {
                if (_RESETS.ContainsKey(id)) throw new DuplicateNameException($"Reset function {id} already registered");
                if (_TICKS.ContainsKey(id)) throw new DuplicateNameException($"Tick function {id} already registered");
                if (_ACCESS_TYPES.ContainsKey(id)) throw new DuplicateNameException($"Access types {id} already registered");

                _RESETS[id] = reset;
                _TICKS[id] = tick;
                _ACCESS_TYPES[id] = types;
            }
        }
        
        public static NodeState Tick(INodeBlob blob, IBlackboard bb)
        {
            return Tick(0, blob, bb);
        }
        
        public static NodeState Tick(int index, INodeBlob blob, IBlackboard bb)
        {
            var typeId = blob.GetTypeId(index);
            var state = _TICKS[typeId](index, blob, bb);
            return state;
        }

        public static void Reset(int index, INodeBlob blob, IBlackboard bb)
        {
            var typeId = blob.GetTypeId(index);
            _RESETS[typeId](index, blob, bb);
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