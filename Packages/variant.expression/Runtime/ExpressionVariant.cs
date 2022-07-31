using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Blob;
using EntitiesBT.Core;
using Nuwa;
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
            public BlobString<UTF8Encoding> Expression;
            public int ExpressionType;
            public BlobArray<BlobVariantPtrRO> Variants;
            public BlobArray<BlobString<UTF8Encoding>> VariantNames;
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

            public void Allocate(BlobVariantStream stream)
            {
                stream.SetVariantId(GuidHashCode(GUID));
                var dataBuilder = new StructBuilder<Data>();
                dataBuilder.SetString(ref dataBuilder.Value.Expression, _expression);
                dataBuilder.SetValue(ref dataBuilder.Value.ExpressionType, VariantValueTypeRegistry.GetIdByType(typeof(T)));
                dataBuilder.SetArray(
                    ref dataBuilder.Value.Variants,
                    _sources.Select(source => new VariantBuilder<BlobVariantPtrRO>(source.Value))
                );
                dataBuilder.SetArray(
                    ref dataBuilder.Value.VariantNames,
                    _sources.Select(source => new StringBuilder<UTF8Encoding>(source.Name))
                );
                dataBuilder.SetArray(
                    ref dataBuilder.Value.VariantTypes,
                    _sources.Select(source => VariantValueTypeRegistry.GetIdByType(source.Value.FindValueType()))
                );
                stream.SetVariantValue(dataBuilder);
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