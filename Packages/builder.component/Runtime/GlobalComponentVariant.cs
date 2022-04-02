using System;
using System.Reflection;
using System.Runtime.InteropServices;
using EntitiesBT.Core;
using Nuwa;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using UnityEngine;
using static EntitiesBT.Core.Utilities;

namespace EntitiesBT.Variant
{
    [VariantClass(GUID)]
    public static class GlobalComponentVariant
    {
        public const string GUID = "6AB4A2A7-E99F-4940-9FFA-7E734DE771C9";

        [Serializable]
        public class Any<T> : IVariant<T> where T : unmanaged
        {
            // TODO: check whether `Value` is part of behavior tree or not.
            public GlobalValues Value;

            [FieldName(DeclaringTypeVariable = nameof(_declaringType), FieldTypeVariable = nameof(_fieldType))]
            public string ValueFieldName;

            protected Type _declaringType => Value?.BlobType;
            protected Type _fieldType => typeof(T);

            public void Allocate(IBlobStream stream, ref BlobVariant blobVariant)
            {
                if (Value == null || Value.BlobType == null) throw new ArgumentException();
                blobVariant.VariantId = GuidHashCode(GUID);
                var fieldInfo = Value.BlobType.GetField(ValueFieldName, BindingFlags.Instance | BindingFlags.Public);
                if (fieldInfo == null || fieldInfo.FieldType != typeof(T))
                {
                    Debug.LogError($"{Value.BlobType.Name}.{ValueFieldName} is not valid", Value);
                    throw new ArgumentException();
                }
                var valueOffset = Value.Offset + Marshal.OffsetOf(Value.BlobType, ValueFieldName).ToInt32();
                return stream.Allocate(ref blobVariant, valueOffset);
            }

            public object PreviewValue => Value?.GetPreviewValue(ValueFieldName);
        }

        [Serializable] public class Reader<T> : Any<T>, IVariantReader<T> where T : unmanaged {}
        [Serializable] public class Writer<T> : Any<T>, IVariantWriter<T> where T : unmanaged {}
        [Serializable] public class ReaderAndWriter<T> : Any<T>, IVariantReaderAndWriter<T> where T : unmanaged {}

        [RefReaderMethod]
        private static unsafe ref T Read<T, TNodeBlob, TBlackboard>(ref BlobVariant blobVariant, int index, ref TNodeBlob blob, ref TBlackboard bb)
            where T : unmanaged
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
            var valueOffset = blobVariant.As<int>();
            var valuePtr = blob.GetRuntimeScopeValuePtr(valueOffset);
            return ref UnsafeUtility.AsRef<T>(valuePtr.ToPointer());
        }

        [ReadWritePointerMethod]
        private static IntPtr GetPointer<TNodeBlob, TBlackboard>(ref BlobVariant blobVariant, int index, ref TNodeBlob blob, ref TBlackboard bb)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
            var valueOffset = blobVariant.As<int>();
            return blob.GetRuntimeScopeValuePtr(valueOffset);
        }
    }
}