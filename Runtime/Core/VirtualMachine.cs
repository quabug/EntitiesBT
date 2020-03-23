using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using UnityEngine.Scripting;

namespace EntitiesBT.Core
{
    public static class VirtualMachine
    {

        delegate NodeState TickFunc(IntPtr ptr, int index, INodeBlob blob, IBlackboard bb);
        delegate void ResetFunc(IntPtr ptr, int index, INodeBlob blob, IBlackboard bb);
        delegate IEnumerable<ComponentType> AccessTypesFunc(IntPtr ptr, int index, INodeBlob blob);
        
        static unsafe class NodeMethodDispatcher<T> where T : struct, INodeData
        {
            [Preserve]
            public static NodeState Tick(IntPtr ptr, int index, INodeBlob blob, IBlackboard bb)
            {
                return UnsafeUtilityEx.AsRef<T>((void*)ptr).Tick(index, blob, bb);
            }
            
            [Preserve]
            public static void Reset(IntPtr ptr, int index, INodeBlob blob, IBlackboard bb)
            {
                UnsafeUtilityEx.AsRef<T>((void*)ptr).Reset(index, blob, bb);
            }

            [Preserve]
            public static IEnumerable<ComponentType> AccessTypes(IntPtr ptr, int index, INodeBlob blob)
            {
                return UnsafeUtilityEx.AsRef<T>((void*)ptr).AccessTypes(index, blob);
            }
        }
        
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
            var ptr = blob.GetRuntimeDataPtr(index);
            return _NODES[typeId].AccessTypes.Invoke(ptr, index, blob);
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
                var resetFunc = CreateDelegate<ResetFunc>("Reset");
                var tickFunc = CreateDelegate<TickFunc>("Tick");
                var accessTypes = CreateDelegate<AccessTypesFunc>("AccessTypes");

                T CreateDelegate<T>(string methodName) where T : Delegate
                {
                    return (T) typeof(NodeMethodDispatcher<>)
                        .MakeGenericType(type)
                        .GetMethod(methodName, BindingFlags.Public | BindingFlags.Static)
                        .CreateDelegate(typeof(T))
                    ;
                }
                
                if (_NODES.ContainsKey(attribute.Id)) throw new DuplicateNameException($"Node {type}[{attribute.Id}] already registered");
                _NODES[attribute.Id] = new Node(type, resetFunc, tickFunc, accessTypes);
            }
        }
        
        public static NodeState Tick(INodeBlob blob, IBlackboard bb)
        {
            return Tick(0, blob, bb);
        }
        
        public static NodeState Tick(int index, INodeBlob blob, IBlackboard bb)
        {
            var typeId = blob.GetTypeId(index);
            var ptr = blob.GetRuntimeDataPtr(index);
            var state = _NODES[typeId].Tick.Invoke(ptr, index, blob, bb);
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
                var ptr = blob.GetRuntimeDataPtr(i);
                _NODES[typeId].Reset.Invoke(ptr, i, blob, bb);
            }
        }

        public static void Reset(INodeBlob blob, IBlackboard bb)
        {
            var count = blob.GetEndIndex(0);
            Reset(0, blob, bb, count);
        }
    }
}