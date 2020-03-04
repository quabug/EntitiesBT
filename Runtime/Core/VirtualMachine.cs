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
        public delegate IEnumerable<ComponentType> AccessTypesFunc(int nodeIndex, INodeBlob blob);

        readonly struct Node
        {
            public readonly Type Type;
            public readonly ResetFunc Reset;
            public readonly TickFunc Tick;
            public readonly AccessTypesFunc AccessTypes;

            public Node(Type type, ResetFunc reset, TickFunc tick, AccessTypesFunc accessTypes)
            {
                Type = type;
                Reset = reset;
                Tick = tick;
                AccessTypes = accessTypes;
            }
        }
        
        
        private static readonly Dictionary<int, Node> _NODES = new Dictionary<int, Node>();

        public static IEnumerable<ComponentType> GetAccessTypes(int index, INodeBlob blob)
        {
            var typeId = blob.GetTypeId(index);
            return _NODES[typeId].AccessTypes(index, blob);
        }
        
        public static Type GetNodeType(int nodeId)
        {
            return _NODES[nodeId].Type;
        }

        static VirtualMachine()
        {
            foreach (var type in AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes()))
            {
                var attribute = type.GetCustomAttribute<BehaviorNodeAttribute>();
                if (attribute == null) continue;
                var resetFunc = GetResetFunc(type.GetMethod(attribute.ResetFunc));
                var tickFunc = GetTickFunc(type.GetMethod(attribute.TickFunc));
                var accessTypes = GetAccessTypes(type.GetMethod(attribute.AccessTypesFunc));
                Register(attribute.Id, type, resetFunc, tickFunc, accessTypes);
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
            
            AccessTypesFunc GetAccessTypes(MethodInfo methodInfo)
            {
                return methodInfo == null
                    ? (index, blob) => Enumerable.Empty<ComponentType>()
                    : (AccessTypesFunc)methodInfo.CreateDelegate(typeof(AccessTypesFunc))
                ;
            }

            void Register(int id, Type type, ResetFunc reset, TickFunc tick, AccessTypesFunc types)
            {
                if (_NODES.ContainsKey(id)) throw new DuplicateNameException($"Node {id} already registered");
                _NODES[id] = new Node(type, reset, tick, types);
            }
        }
        
        public static NodeState Tick(INodeBlob blob, IBlackboard bb)
        {
            return Tick(0, blob, bb);
        }
        
        public static NodeState Tick(int index, INodeBlob blob, IBlackboard bb)
        {
            var typeId = blob.GetTypeId(index);
            var state = _NODES[typeId].Tick(index, blob, bb);
            blob.SetState(index, state);
            return state;
        }

        public static void Reset(int fromIndex, INodeBlob blob, IBlackboard bb, int count = 1)
        {
            blob.ResetStates(fromIndex, count);
            blob.ResetRuntimeData(fromIndex, count);
            for (var i = fromIndex; i < fromIndex + count; i++)
            {
                var typeId = blob.GetTypeId(i);
                _NODES[typeId].Reset(i, blob, bb);
            }
        }

        public static void Reset(INodeBlob blob, IBlackboard bb)
        {
            var count = blob.GetEndIndex(0);
            Reset(0, blob, bb, count);
        }
    }
}