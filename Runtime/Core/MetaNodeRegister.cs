using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using UnityEngine.Scripting;

namespace EntitiesBT.Core
{
    internal static class MetaNodeRegister
    {
        internal delegate NodeState TickFunc(IntPtr ptr, int index, INodeBlob blob, IBlackboard bb);
        
        [Preserve]
        private static unsafe NodeState Tick<T>(IntPtr ptr, int index, INodeBlob blob, IBlackboard bb) where T : struct, INodeData
        {
            return UnsafeUtilityEx.AsRef<T>((void*)ptr).Tick(index, blob, bb);
        }
        
        internal delegate void ResetFunc(IntPtr ptr, int index, INodeBlob blob, IBlackboard bb);
        
        [Preserve]
        private static unsafe void Reset<T>(IntPtr ptr, int index, INodeBlob blob, IBlackboard bb) where T : struct, INodeData
        {
            UnsafeUtilityEx.AsRef<T>((void*)ptr).Reset(index, blob, bb);
        }
        
        internal delegate IEnumerable<ComponentType> ComponentTypesFunc(IntPtr ptr);
        
        [Preserve]
        private static unsafe IEnumerable<ComponentType> ReadOnlyTypes<T>(IntPtr ptr) where T : struct, IRuntimeComponentAccessor
        {
            return UnsafeUtilityEx.AsRef<T>((void*)ptr).ComponentAccessList.Select(t => ComponentType.ReadOnly(t.TypeIndex));
        }
        
        [Preserve]
        private static unsafe IEnumerable<ComponentType> ReadWriteTypes<T>(IntPtr ptr) where T : struct, IRuntimeComponentAccessor
        {
            return UnsafeUtilityEx.AsRef<T>((void*)ptr).ComponentAccessList;
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
            public ResetFunc Reset;
            public TickFunc Tick;
            public ComponentType[] StaticTypes;
            public RuntimeTypeAccessor[] RuntimeTypes;
        }
        
        internal static readonly Dictionary<int, Node> NODES = new Dictionary<int, Node>();

        static MetaNodeRegister()
        {
            foreach (var type in AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes()))
            {
                var attribute = type.GetCustomAttribute<BehaviorNodeAttribute>();
                if (attribute == null) continue;
                if (NODES.ContainsKey(attribute.Id)) throw new DuplicateNameException($"Node {type}[{attribute.Id}] already registered");

                NODES[attribute.Id] = new Node{
                    Type = type
                  , Reset = CreateDelegate<ResetFunc>("Reset", type)
                  , Tick = CreateDelegate<TickFunc>("Tick", type)
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
}
