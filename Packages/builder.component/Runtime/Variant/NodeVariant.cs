using System;
using System.Reflection;
using System.Runtime.InteropServices;
using EntitiesBT.Components;
using EntitiesBT.Core;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Scripting;
using static EntitiesBT.Core.Utilities;

namespace EntitiesBT.Variant
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class VariantNodeObjectAttribute : PropertyAttribute
    {
        public string NodeObjectFieldName;
        public VariantNodeObjectAttribute(string nodeObjectFieldName)
        {
            NodeObjectFieldName = nodeObjectFieldName;
        }
    }

    public class NodeVariant
    {
        // TODO: check loop ref?
        [Serializable]
        public class Any<T> : IVariant where T : unmanaged
        {
            public BTNode NodeObject;
            [VariantNodeObject(nameof(NodeObject))] public string ValueFieldName;

            public unsafe IntPtr Allocate(ref BlobBuilder builder, ref BlobVariant blobVariant, INodeDataBuilder self, ITreeNode<INodeDataBuilder>[] tree)
            {
                return Allocate<T>(ref builder, ref blobVariant, self, tree, NodeObject, ValueFieldName);
            }
        }

        [Serializable] public class Reader<T> : Any<T>, IVariantReader<T> where T : unmanaged {}
        [Serializable] public class Writer<T> : Any<T>, IVariantWriter<T> where T : unmanaged {}
        [Serializable] public class ReaderAndWriter<T> : Any<T>, IVariantReaderAndWriter<T> where T : unmanaged {}

        public const string ID_RUNTIME_NODE = "220681AA-D884-4E87-90A8-5A8657A734BD";
        public const string ID_RUNTIME_NODE_VARIANT = "7DE6DCA5-71DF-4145-91A6-17EB813B9DEB";

        [Preserve, WriterMethod(ID_RUNTIME_NODE_VARIANT)]
        private static unsafe void WriteVariableFunc<T, TNodeBlob, TBlackboard>(ref BlobVariant blobVariant, int index, ref TNodeBlob blob, ref TBlackboard bb, T value)
            where T : unmanaged
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
            ref var data = ref blobVariant.Value<DynamicNodeRefData>();
            var ptr = blob.GetRuntimeDataPtr(data.Index);
            ref var variable = ref UnsafeUtility.AsRef<BlobVariantReader<T>>(IntPtr.Add(ptr, data.Offset).ToPointer());
            // TODO: check writable on editor?
            variable.Value.WriteWithRefFallback(index, ref blob, ref bb, value);
        }

        [Preserve, RefReaderMethod(ID_RUNTIME_NODE)]
        private static ref T GetRuntimeNodeData<T, TNodeBlob, TBlackboard>(ref BlobVariant blobVariant, int index, ref TNodeBlob blob, ref TBlackboard bb)
            where T : unmanaged
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
            return ref GetValue<T>(ref blobVariant, blob.GetRuntimeDataPtr);
        }

        [Preserve, ReaderMethod(ID_RUNTIME_NODE_VARIANT)]
        private static unsafe T GetRuntimeNodeVariable<T, TNodeBlob, TBlackboard>(ref BlobVariant blobVariant, int index, ref TNodeBlob blob, ref TBlackboard bb)
            where T : unmanaged
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
            ref var data = ref blobVariant.Value<DynamicNodeRefData>();
            var ptr = blob.GetRuntimeDataPtr(data.Index);
            ref var variable = ref UnsafeUtility.AsRef<BlobVariantReader<T>>(IntPtr.Add(ptr, data.Offset).ToPointer());
            return variable.Read(index, ref blob, ref bb);
        }

        private static unsafe ref T GetValue<T>(ref BlobVariant blobVariant, Func<int, IntPtr> ptrFunc) where T : unmanaged
        {
            ref var data = ref blobVariant.Value<DynamicNodeRefData>();
            var ptr = ptrFunc(data.Index);
            return ref UnsafeUtility.AsRef<T>(IntPtr.Add(ptr, data.Offset).ToPointer());
        }

        private struct DynamicNodeRefData
        {
            public int Index;
            public int Offset;
        }

        public static IntPtr Allocate<T>(
            ref BlobBuilder builder
            , ref BlobVariant blobVariant
            , INodeDataBuilder self
            , ITreeNode<INodeDataBuilder>[] tree
            , INodeDataBuilder nodeObject
            , string valueFieldName
        ) where T : unmanaged
        {
            var index = Array.FindIndex(tree, node => ReferenceEquals(node.Value, nodeObject));
            if (nodeObject == null || index < 0)
            {
                Debug.LogError($"Invalid `NodeObject` {nodeObject}", (UnityEngine.Object)self);
                throw new ArgumentException();
            }

            var nodeType = VirtualMachine.GetNodeType(nodeObject.NodeId);
            if (string.IsNullOrEmpty(valueFieldName) && nodeType == typeof(T))
                return builder.Allocate(ref blobVariant, new DynamicNodeRefData{ Index = index, Offset = 0});

            var fieldInfo = nodeType.GetField(valueFieldName, BindingFlags.Instance | BindingFlags.Public);
            if (fieldInfo == null)
            {
                Debug.LogError($"Invalid `ValueFieldName` {valueFieldName}", (UnityEngine.Object)self);
                throw new ArgumentException();
            }

            var fieldType = fieldInfo.FieldType;
            if (fieldType == typeof(T))
            {
                blobVariant.VariantId = GuidHashCode(ID_RUNTIME_NODE);
            }
            else if (fieldType == typeof(BlobVariantReader<T>) || fieldType == typeof(BlobVariantReaderAndWriter<T>))
            {
                blobVariant.VariantId = GuidHashCode(ID_RUNTIME_NODE_VARIANT);
            }
            else
            {
                Debug.LogError($"Invalid type of `ValueFieldName` {valueFieldName} {fieldType}", (UnityEngine.Object)self);
                throw new ArgumentException();
            }

            var fieldOffset = Marshal.OffsetOf(nodeType, valueFieldName).ToInt32();
            if (fieldType == typeof(BlobVariantReaderAndWriter<T>))
                fieldOffset += Marshal.OffsetOf(typeof(BlobVariantReaderAndWriter<T>) , nameof(BlobVariantReaderAndWriter<T>.Reader)).ToInt32();
            return builder.Allocate(ref blobVariant, new DynamicNodeRefData{ Index = index, Offset = fieldOffset});
        }
    }
}
