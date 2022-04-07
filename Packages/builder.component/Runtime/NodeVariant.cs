using System;
using System.Reflection;
using EntitiesBT.Components;
using EntitiesBT.Core;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
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

    [VariantClass(ID_RUNTIME_NODE_VARIANT)]
    public class NodeVariant
    {
        public const string ID_RUNTIME_NODE = "220681AA-D884-4E87-90A8-5A8657A734BD";
        public const string ID_RUNTIME_NODE_VARIANT = "7DE6DCA5-71DF-4145-91A6-17EB813B9DEB";

        // TODO: check loop ref?
        [Serializable]
        public class Any<T> : IVariant where T : unmanaged
        {
            public BTNode NodeObject;
            [VariantNodeObject(nameof(NodeObject))] public string ValueFieldName;

            public void Allocate(BlobVariantStream stream)
            {
                Allocate<T>(stream, NodeObject, ValueFieldName);
            }

            public object PreviewValue => null;
        }

        [Serializable] public class Reader<T> : Any<T>, IVariantReader<T> where T : unmanaged {}
        [Serializable] public class Writer<T> : Any<T>, IVariantWriter<T> where T : unmanaged {}
        [Serializable] public class ReaderAndWriter<T> : Any<T>, IVariantReaderAndWriter<T> where T : unmanaged {}

        [RefReaderMethod(OverrideGuid = ID_RUNTIME_NODE)]
        private static ref T GetRuntimeNodeData<T, TNodeBlob, TBlackboard>(ref BlobVariant blobVariant, int index, ref TNodeBlob blob, ref TBlackboard bb)
            where T : unmanaged
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
            return ref GetValue<T>(ref blobVariant, blob.GetRuntimeDataPtr);
        }

        [ReadOnlyPointerMethod(OverrideGuid = ID_RUNTIME_NODE)]
        private static IntPtr GetRuntimeNodeDataPointerRO<TNodeBlob, TBlackboard>(ref BlobVariant blobVariant, int index, ref TNodeBlob blob, ref TBlackboard bb)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
            return GetValuePtr(ref blobVariant, blob.GetRuntimeDataPtr);
        }

        [WriterMethod]
        private static void WriteVariableFunc<T, TNodeBlob, TBlackboard>(ref BlobVariant blobVariant, int index, ref TNodeBlob blob, ref TBlackboard bb, T value)
            where T : unmanaged
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
            ref var variant = ref GetVariant<T, TNodeBlob>(ref blobVariant, ref blob);
            // TODO: check writable on editor?
            variant.Value.WriteWithRefFallback(index, ref blob, ref bb, value);
        }

        [ReaderMethod]
        private static T GetRuntimeNodeVariable<T, TNodeBlob, TBlackboard>(ref BlobVariant blobVariant, int index, ref TNodeBlob blob, ref TBlackboard bb)
            where T : unmanaged
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
            ref var variant = ref GetVariant<T, TNodeBlob>(ref blobVariant, ref blob);
            return variant.Read(index, ref blob, ref bb);
        }

        [ReadOnlyPointerMethod]
        private static IntPtr GetRuntimeNodeVariantPointerRO<TNodeBlob, TBlackboard>(ref BlobVariant blobVariant, int index, ref TNodeBlob blob, ref TBlackboard bb)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
            ref var variant = ref GetVariant(ref blobVariant, ref blob);
            return variant.ReadOnlyPtrWithReadWriteFallback(index, ref blob, ref bb);
        }

        private static unsafe ref T GetValue<T>(ref BlobVariant blobVariant, Func<int, IntPtr> ptrFunc) where T : unmanaged
        {
            return ref UnsafeUtility.AsRef<T>(GetValuePtr(ref blobVariant, ptrFunc).ToPointer());
        }

        private static IntPtr GetValuePtr(ref BlobVariant blobVariant, Func<int, IntPtr> ptrFunc)
        {
            ref var data = ref blobVariant.As<DynamicNodeRefData>();
            var ptr = ptrFunc(data.Index);
            return IntPtr.Add(ptr, data.Offset);
        }

        private struct DynamicNodeRefData
        {
            public int Index;
            public int Offset;
        }

        private static unsafe ref BlobVariantRO<T> GetVariant<T, TNodeBlob>(ref BlobVariant blobVariant, ref TNodeBlob blob)
            where T : unmanaged
            where TNodeBlob : struct, INodeBlob
        {
            return ref UnsafeUtility.AsRef<BlobVariantRO<T>>(GetVariantPtr(ref blobVariant, ref blob).ToPointer());
        }

        private static unsafe ref BlobVariant GetVariant<TNodeBlob>(ref BlobVariant blobVariant, ref TNodeBlob blob) where TNodeBlob : struct, INodeBlob
        {
            return ref UnsafeUtility.AsRef<BlobVariant>(GetVariantPtr(ref blobVariant, ref blob).ToPointer());
        }

        private static IntPtr GetVariantPtr<TNodeBlob>(ref BlobVariant blobVariant, ref TNodeBlob blob) where TNodeBlob : struct, INodeBlob
        {
            ref var data = ref blobVariant.As<DynamicNodeRefData>();
            var ptr = blob.GetRuntimeDataPtr(data.Index);
            return IntPtr.Add(ptr, data.Offset);
        }

        public static void Allocate<T>(
            BlobVariantStream stream
            , INodeDataBuilder nodeObject
            , string valueFieldName
        ) where T : unmanaged
        {
            var index = nodeObject.NodeIndex;
            if (nodeObject == null || index < 0)
            {
                Debug.LogError($"Invalid `NodeObject` {nodeObject}");
                throw new ArgumentException();
            }

            var nodeType = VirtualMachine.GetNodeType(nodeObject.NodeId);
            if (string.IsNullOrEmpty(valueFieldName) && nodeType == typeof(T))
            {
                stream.SetVariantValue(new DynamicNodeRefData { Index = index, Offset = 0 });
                return;
            }

            var fieldInfo = nodeType.GetField(valueFieldName, BindingFlags.Instance | BindingFlags.Public);
            if (fieldInfo == null)
            {
                Debug.LogError($"Invalid `ValueFieldName` {valueFieldName}");
                throw new ArgumentException();
            }

            var fieldType = fieldInfo.FieldType;
            if (fieldType == typeof(T))
            {
                stream.SetVariantId(GuidHashCode(ID_RUNTIME_NODE));
            }
            else if (fieldType == typeof(BlobVariantRO<T>) || fieldType == typeof(BlobVariantRW<T>))
            {
                stream.SetVariantId(GuidHashCode(ID_RUNTIME_NODE_VARIANT));
            }
            else
            {
                Debug.LogError($"Invalid type of `ValueFieldName` {valueFieldName} {fieldType}");
                throw new ArgumentException();
            }

            var fieldOffset = UnsafeUtility.GetFieldOffset(fieldInfo);
            if (fieldType == typeof(BlobVariantRW<T>))
            {
                var fi = typeof(BlobVariantRW<T>).GetField(nameof(BlobVariantRW<T>.Reader),
                    BindingFlags.Instance | BindingFlags.Public)
                ;
                fieldOffset += UnsafeUtility.GetFieldOffset(fi);
            }
            stream.SetVariantValue(new DynamicNodeRefData{ Index = index, Offset = fieldOffset});
        }
    }
}
