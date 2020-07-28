using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using UnityEngine.Scripting;
using static EntitiesBT.Core.Utilities;

namespace EntitiesBT.Core
{
    internal static class MetaNodeRegister
    {
        internal delegate IEnumerable<ComponentType> ComponentTypesFunc(IntPtr ptr);
        
        [Preserve]
        private static unsafe IEnumerable<ComponentType> ReadOnlyTypes<T>(IntPtr ptr) where T : struct, IRuntimeComponentAccessor
        {
            return UnsafeUtility.AsRef<T>((void*)ptr).ComponentAccessList.Select(t => ComponentType.ReadOnly(t.TypeIndex));
        }
        
        [Preserve]
        private static unsafe IEnumerable<ComponentType> ReadWriteTypes<T>(IntPtr ptr) where T : struct, IRuntimeComponentAccessor
        {
            return UnsafeUtility.AsRef<T>((void*)ptr).ComponentAccessList;
        }

        internal readonly struct RuntimeTypeAccessor
        {
            public readonly int Offset;
            public readonly ComponentTypesFunc Func;

            public RuntimeTypeAccessor(int offset, ComponentTypesFunc func)
            {
                Offset = offset;
                Func = func;
            }
        }
        
        internal class Node
        {
            public Type Type;
            public ComponentType[] StaticTypes;
            public RuntimeTypeAccessor[] RuntimeTypes;
        }
        
        internal static readonly Dictionary<int, Node> NODES = new Dictionary<int, Node>();

        static MetaNodeRegister()
        {
            foreach (var type in BEHAVIOR_TREE_ASSEMBLY_TYPES.Value)
            {
                var attribute = type.GetCustomAttribute<BehaviorNodeAttribute>();
                if (attribute == null) continue;
                if (NODES.ContainsKey(attribute.Id)) throw new DuplicateNameException($"Node {type}[{attribute.Id}] already registered");

                NODES[attribute.Id] = new Node{
                    Type = type
                  , StaticTypes = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
                        .SelectMany(mi => mi.GetCustomAttributes<ComponentAccessorAttribute>())
                        .SelectMany(attr => attr.Types)
                        .ToArray()
                  , RuntimeTypes = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                        .Where(fi => typeof(IRuntimeComponentAccessor).IsAssignableFrom(fi.FieldType))
                        .Where(fi => fi.GetCustomAttribute<OptionalAttribute>() == null)
                        .Select(fi => new RuntimeTypeAccessor(
                            Marshal.OffsetOf(type, fi.Name).ToInt32()
                          , fi.GetCustomAttribute<ReadOnlyAttribute>() == null || fi.GetCustomAttribute<ReadWriteAttribute>() != null
                                ? CreateDelegate<ComponentTypesFunc>("ReadWriteTypes", fi.FieldType)
                                : CreateDelegate<ComponentTypesFunc>("ReadOnlyTypes", fi.FieldType)
                        )).ToArray()
                };
            }
            
            T CreateDelegate<T>(string methodName, Type type) where T : Delegate
            {
                return (T) typeof(MetaNodeRegister)
                    .GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Static)
                    .MakeGenericMethod(type)
                    .CreateDelegate(typeof(T))
                ;
            }
        }
    }
    internal static class MetaNodeRegister<TNodeBlob, TBlackboard>
        where TNodeBlob : struct, INodeBlob
        where TBlackboard : struct, IBlackboard
    {
        internal delegate NodeState TickFunc(IntPtr ptr, int index, TNodeBlob blob, TBlackboard bb);
        
        [Preserve]
        private static unsafe NodeState Tick<TNodeData>(IntPtr ptr, int index, TNodeBlob blob, TBlackboard bb)
            where TNodeData : struct, INodeData
        {
            return UnsafeUtility.AsRef<TNodeData>((void*)ptr).Tick(index, ref blob, ref bb);
        }
        
        internal delegate void ResetFunc(IntPtr ptr, int index, TNodeBlob blob, TBlackboard bb);
        
        [Preserve]
        private static unsafe void Reset<TNodeData>(IntPtr ptr, int index, TNodeBlob blob, TBlackboard bb)
            where TNodeData : struct, INodeData
        {
            UnsafeUtility.AsRef<TNodeData>((void*)ptr).Reset(index, ref blob, ref bb);
        }
        
        internal class Node
        {
            public ResetFunc Reset;
            public TickFunc Tick;
        }
        
        internal static readonly Dictionary<int, Node> NODES = new Dictionary<int, Node>();

        static MetaNodeRegister()
        {
            foreach (var type in MetaNodeRegister.NODES.Values.Select(node => node.Type))
            {
                var attribute = type.GetCustomAttribute<BehaviorNodeAttribute>();
                NODES[attribute.Id] = new Node {
                  Reset = CreateDelegate<ResetFunc>("Reset", type)
                  , Tick = CreateDelegate<TickFunc>("Tick", type)
                };
            }
            
            T CreateDelegate<T>(string methodName, Type type) where T : Delegate
            {
                return (T) typeof(MetaNodeRegister<TNodeBlob, TBlackboard>)
                    .GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Static)
                    .MakeGenericMethod(type)
                    .CreateDelegate(typeof(T))
                ;
            }
        }
    }
}
