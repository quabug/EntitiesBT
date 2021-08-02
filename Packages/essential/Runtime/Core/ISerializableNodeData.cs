using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;

namespace EntitiesBT.Core
{
    public interface ISerializableNodeData
    {
        Type NodeType { get; }

        unsafe void Build(void* dataPtr, BlobBuilder builder, INodeDataBuilder self, ITreeNode<INodeDataBuilder>[] tree);
        unsafe void Load(void* dataPtr);
    }

    [Serializable]
    public abstract class SerializableNodeData<T> : ISerializableNodeData where T : unmanaged, INodeData
    {
        public Type NodeType => typeof(T);

        unsafe void ISerializableNodeData.Build(void* dataPtr, BlobBuilder builder, INodeDataBuilder self, ITreeNode<INodeDataBuilder>[] tree)
        {
            Build(ref UnsafeUtility.AsRef<T>(dataPtr), builder, self, tree);
        }

        public unsafe void Load(void* dataPtr)
        {
            Load(ref UnsafeUtility.AsRef<T>(dataPtr));
        }

        protected virtual void Build(ref T data, BlobBuilder builder, INodeDataBuilder self, ITreeNode<INodeDataBuilder>[] tree) {}
        protected virtual void Load(ref T data) {}
    }

    public static class SerializableNodeDataRegistry
    {
        private static readonly IReadOnlyDictionary<int, Type> _serializables;

        static SerializableNodeDataRegistry()
        {
            _serializables = (
                from assembly in AppDomain.CurrentDomain.GetAssemblies()
                from type in assembly.GetTypesWithoutException()
                where !type.IsAbstract && typeof(ISerializableNodeData).IsAssignableFrom(type)
                from attribute in GetNodeDataType(type).GetCustomAttributes<BehaviorNodeAttribute>()
                select (attribute.Id, type)
            ).ToDictionary(t => t.Id, t => t.type);
        }

        private static Type GetNodeDataType(Type serializableType)
        {
            for (;;)
            {
                if (serializableType == null) return null;
                if (serializableType.IsGenericType && serializableType.GetGenericTypeDefinition() == typeof(SerializableNodeData<>))
                    return serializableType.GenericTypeArguments[0];
                serializableType = serializableType.BaseType;
            }
        }

        public static Type FindSerializableType<TNodeData>() where TNodeData : INodeData
        {
            return FindSerializableType(typeof(TNodeData));
        }

        public static Type FindSerializableType(int id)
        {
            _serializables.TryGetValue(id, out var type);
            return type;
        }

        public static Type FindSerializableType(Type nodeDataType)
        {
            var id = nodeDataType.GetCustomAttribute<BehaviorNodeAttribute>().Id;
            return FindSerializableType(id);
        }
    }
}