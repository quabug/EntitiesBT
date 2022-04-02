using System;
using System.Collections.Concurrent;
using System.Runtime.InteropServices;
using EntitiesBT.Core;
using Nuwa;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using UnityEngine;
using static EntitiesBT.Core.Utilities;

namespace EntitiesBT.Variant.Expression
{
    [VariantClass(GUID)]
    public static class ExpressionVariant
    {
        public const string GUID = "D7492D8C-CCE3-4071-85C6-92E7EE1E9C48";

        public struct Data
        {
            public BlobString Expression;
            public int ExpressionType;
            public BlobArray<BlobVariantPtrRO> Variants;
            public BlobArray<BlobString> VariantNames;
            public BlobArray<int> VariantTypes;
        }

        private static readonly ConcurrentDictionary<LambdaId, object[]> _expressionParameters = new ConcurrentDictionary<LambdaId, object[]>();

        [Serializable]
        public class Reader<T> : IVariantReader<T> where T : unmanaged
        {
            [Serializable]
            internal class Variant
            {
                [SerializeReference]
                [SerializeReferenceDrawer(RenamePatter = @"^.*(\.|\+|/)(\w+)$||$2")]
                public IVariantReader Value;
                public string Name;
            }

            [SerializeField] internal string _expression;
            [SerializeField] internal Variant[] _sources;

            public unsafe void Allocate(IBlobStream stream, ref BlobVariant blobVariant)
            {
                blobVariant.VariantId = GuidHashCode(GUID);
                ref var blobPtr = ref UnsafeUtility.As<int, BlobPtr<Data>>(ref blobVariant.MetaDataOffsetPtr);
                ref var data = ref stream.Allocate(ref blobPtr);
                data.ExpressionType = VariantValueTypeRegistry.GetIdByType(typeof(T));
                stream.AllocateString(ref data.Expression, _expression);
                var variants = stream.Allocate(ref data.Variants, _sources.Length);
                var names = stream.Allocate(ref data.VariantNames, _sources.Length);
                var types = stream.Allocate(ref data.VariantTypes, _sources.Length);
                for (var i = 0; i < _sources.Length; i++)
                {
                    _sources[i].Value.Allocate(ref stream, ref variants[i]);
                    stream.AllocateString(ref names[i], _sources[i].Name);
                    types[i] = VariantValueTypeRegistry.GetIdByType(_sources[i].Value.FindValueType());
                }
                return new IntPtr(UnsafeUtility.AddressOf(ref data));
            }

            public object PreviewValue => null;
        }

        [ReaderMethod]
        private static T Read<T, TNodeBlob, TBlackboard>(ref BlobVariant blobVariant, int index, ref TNodeBlob blob, ref TBlackboard bb)
            where T : unmanaged
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
            ref var data = ref blobVariant.As<Data>();
            var lambdaId = new LambdaId(blob.RuntimeId, index);
            var lambda = data.Parse(lambdaId);

            if (!_expressionParameters.TryGetValue(lambdaId, out var arguments))
            {
                arguments = new object[data.Variants.Length];
                _expressionParameters[lambdaId] = arguments;
            }

            for (var i = 0; i < arguments.Length; i++)
            {
                var type = VariantValueTypeRegistry.GetTypeById(data.VariantTypes[i]);
                var pointer = data.Variants[i].GetPointer(index, ref blob, ref bb);
                arguments[i] = Marshal.PtrToStructure(pointer, type);
            }
            return (T) lambda.Invoke(arguments);
        }
    }
}