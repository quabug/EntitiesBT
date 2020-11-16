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

    public static class NodeVariant
    {

        public const string ID_RUNTIME_NODE = "220681AA-D884-4E87-90A8-5A8657A734BD";
        public const string ID_DEFAULT_NODE = "743FABC9-77E6-4C2B-A475-ADED2A3B96AD";
        public const string ID_RUNTIME_NODE_VARIABLE = "7DE6DCA5-71DF-4145-91A6-17EB813B9DEB";
        public const string ID_DEFAULT_NODE_VARIABLE = "A98E0FBF-EF5D-4AFC-9997-0E08A569574D";

        // TODO: check loop ref?
        public class Reader<T> : IVariantReader<T> where T : unmanaged
        {
            private struct DynamicNodeRefData
            {
                public int Index;
                public int Offset;
            }

            public BTNode NodeObject;
            [VariantNodeObject("NodeObject")] public string ValueFieldName;
            public bool AccessRuntimeData = true;

            public void Allocate(ref BlobBuilder builder, ref BlobVariant blobVariant, INodeDataBuilder self, ITreeNode<INodeDataBuilder>[] tree)
            {
                var index = Array.FindIndex(tree, node => ReferenceEquals(node.Value, NodeObject));
                if (!NodeObject || index < 0)
                {
                    Debug.LogError($"Invalid `NodeObject` {NodeObject}", (UnityEngine.Object)self);
                    throw new ArgumentException();
                }

                var nodeType = VirtualMachine.GetNodeType(NodeObject.NodeId);
                if (string.IsNullOrEmpty(ValueFieldName) && nodeType == typeof(T))
                {
                    builder.Allocate(ref blobVariant, new DynamicNodeRefData{ Index = index, Offset = 0});
                    return;
                }

                var fieldInfo = nodeType.GetField(ValueFieldName, BindingFlags.Instance | BindingFlags.Public);
                if (fieldInfo == null)
                {
                    Debug.LogError($"Invalid `ValueFieldName` {ValueFieldName}", (UnityEngine.Object)self);
                    throw new ArgumentException();
                }

                var fieldOffset = Marshal.OffsetOf(nodeType, ValueFieldName).ToInt32();
                builder.Allocate(ref blobVariant, new DynamicNodeRefData{ Index = index, Offset = fieldOffset});

                var fieldType = fieldInfo.FieldType;
                if (fieldType == typeof(T))
                    blobVariant.VariantId = GuidHashCode(AccessRuntimeData ? ID_RUNTIME_NODE : ID_DEFAULT_NODE);
                else if (fieldType == typeof(BlobVariantReader<T>))
                    blobVariant.VariantId = GuidHashCode(AccessRuntimeData ? ID_RUNTIME_NODE_VARIABLE : ID_DEFAULT_NODE_VARIABLE);
                else
                {
                    Debug.LogError($"Invalid type of `ValueFieldName` {ValueFieldName} {fieldType}", (UnityEngine.Object)self);
                    throw new ArgumentException();
                }
            }

            [Preserve, ReaderMethod(ID_RUNTIME_NODE)]
            private static T GetRuntimeNodeData<TNodeBlob, TBlackboard>(ref BlobVariant blobVariant, int index, ref TNodeBlob blob, ref TBlackboard bb)
                where TNodeBlob : struct, INodeBlob
                where TBlackboard : struct, IBlackboard
            {
                return GetRuntimeNodeDataRef(ref blobVariant, index, ref blob, ref bb);
            }

            [Preserve, RefReaderMethod(ID_RUNTIME_NODE)]
            private static ref T GetRuntimeNodeDataRef<TNodeBlob, TBlackboard>(ref BlobVariant blobVariant, int index, ref TNodeBlob blob, ref TBlackboard bb)
                where TNodeBlob : struct, INodeBlob
                where TBlackboard : struct, IBlackboard
            {
                return ref GetValue(ref blobVariant, blob.GetRuntimeDataPtr);
            }

            [Preserve, ReaderMethod(ID_DEFAULT_NODE)]
            private static T GetDefaultNodeData<TNodeBlob, TBlackboard>(ref BlobVariant blobVariant, int index, ref TNodeBlob blob, ref TBlackboard bb)
                where TNodeBlob : struct, INodeBlob
                where TBlackboard : struct, IBlackboard
            {
                return GetDefaultNodeDataRef(ref blobVariant, index, ref blob, ref bb);
            }

            [Preserve, RefReaderMethod(ID_DEFAULT_NODE)]
            private static ref T GetDefaultNodeDataRef<TNodeBlob, TBlackboard>(ref BlobVariant blobVariant, int index, ref TNodeBlob blob, ref TBlackboard bb)
                where TNodeBlob : struct, INodeBlob
                where TBlackboard : struct, IBlackboard
            {
                return ref GetValue(ref blobVariant, blob.GetDefaultDataPtr);
            }

            private static unsafe ref T GetValue(ref BlobVariant blobVariant, Func<int, IntPtr> ptrFunc)
            {
                ref var data = ref blobVariant.Value<DynamicNodeRefData>();
                var ptr = ptrFunc(data.Index);
                return ref UnsafeUtility.AsRef<T>(IntPtr.Add(ptr, data.Offset).ToPointer());
            }

            [Preserve, ReaderMethod(ID_RUNTIME_NODE_VARIABLE)]
            private static unsafe T GetRuntimeNodeVariable<TNodeBlob, TBlackboard>(ref BlobVariant blobVariant, int index, ref TNodeBlob blob, ref TBlackboard bb)
                where TNodeBlob : struct, INodeBlob
                where TBlackboard : struct, IBlackboard
            {
                ref var data = ref blobVariant.Value<DynamicNodeRefData>();
                var ptr = blob.GetRuntimeDataPtr(data.Index);
                ref var variable = ref UnsafeUtility.AsRef<BlobVariantReader<T>>(IntPtr.Add(ptr, data.Offset).ToPointer());
                return variable.Read(index, ref blob, ref bb);
            }

            [Preserve, ReaderMethod(ID_DEFAULT_NODE_VARIABLE)]
            private static unsafe T GetDefaultNodeVariable<TNodeBlob, TBlackboard>(ref BlobVariant blobVariant, int index, ref TNodeBlob blob, ref TBlackboard bb)
                where TNodeBlob : struct, INodeBlob
                where TBlackboard : struct, IBlackboard
            {
                ref var data = ref blobVariant.Value<DynamicNodeRefData>();
                var ptr = blob.GetDefaultDataPtr(data.Index);
                ref var variable = ref UnsafeUtility.AsRef<BlobVariantReader<T>>(IntPtr.Add(ptr, data.Offset).ToPointer());
                return variable.Read(index, ref blob, ref bb);
            }
        }

        public class Writer<T> : IVariantWriter<T> where T : unmanaged
        {
            public void Allocate(ref BlobBuilder builder, ref BlobVariant blobVariant, INodeDataBuilder self, ITreeNode<INodeDataBuilder>[] tree)
            {
                throw new NotImplementedException();
            }
        }
    }
}
