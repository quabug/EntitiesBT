using System;
using Blob;
using EntitiesBT.Core;
using EntitiesBT.Variant;
using EntitiesBT.Variant.Expression;
using NUnit.Framework;

namespace EntitiesBT.Test
{
    public class TestExpression
    {
        private MockNodeBlob _nodeBlob;
        private MockBlackboard _bb;

        private struct MockNodeBlob : INodeBlob
        {
            public int RuntimeId { get; set; }
            public int Count { get; }
            public int GetTypeId(int nodeIndex) => throw new NotImplementedException();
            public int GetEndIndex(int nodeIndex) => throw new NotImplementedException();
            public int GetNodeDataSize(int startNodeIndex, int count = 1) => throw new NotImplementedException();
            public NodeState GetState(int nodeIndex) => throw new NotImplementedException();
            public void SetState(int nodeIndex, NodeState state) => throw new NotImplementedException();
            public void ResetStates(int index, int count = 1) => throw new NotImplementedException();
            public IntPtr GetDefaultDataPtr(int nodeIndex) => throw new NotImplementedException();
            public IntPtr GetRuntimeDataPtr(int nodeIndex) => throw new NotImplementedException();
            public IntPtr GetDefaultScopeValuePtr(int offset) => throw new NotImplementedException();
            public IntPtr GetRuntimeScopeValuePtr(int offset) => throw new NotImplementedException();
        }

        private struct MockBlackboard : IBlackboard
        {
            public bool HasData<T>() where T : struct => throw new NotImplementedException();
            public T GetData<T>() where T : struct => throw new NotImplementedException();
            public ref T GetDataRef<T>() where T : struct => throw new NotImplementedException();
            public bool HasData(Type type) => throw new NotImplementedException();
            public IntPtr GetDataPtrRO(Type type) => throw new NotImplementedException();
            public IntPtr GetDataPtrRW(Type type) => throw new NotImplementedException();
            public T GetObject<T>() where T : class => throw new NotImplementedException();
        }

        [SetUp]
        public void SetUp()
        {
            _nodeBlob = new MockNodeBlob { RuntimeId = 123 };
            _bb = new MockBlackboard();
        }

        [Test]
        public void should_eval_expression()
        {
            var expression = new ExpressionVariant.Reader<float>();
            expression._expression = "(x+y)*2+10";
            expression._sources = new[]
            {
                new ExpressionVariant.Reader<float>.Variant {Value = new LocalVariant.Reader<float> {Value = 4.1f}, Name = "x"},
                new ExpressionVariant.Reader<float>.Variant {Value = new LocalVariant.Reader<int> {Value = 5}, Name = "y"},
            };

            var builder = new VariantBuilder<BlobVariant>(expression);
            using var variant = builder.CreateManagedBlobAssetReference();
            ref var expressionData = ref variant.Value.As<ExpressionVariant.Data>();
            Assert.That(expressionData.Expression.ToString(), Is.EqualTo(expression._expression));
            Assert.That(expressionData.ExpressionType, Is.EqualTo(VariantValueTypeRegistry.GetIdByType(typeof(float))));
            for (var i = 0; i < expression._sources.Length; i++)
            {
                var source = expression._sources[i];
                Assert.That(expressionData.VariantNames[i].ToString(), Is.EqualTo(source.Name));
                Assert.That(expressionData.VariantTypes[i], Is.EqualTo(VariantValueTypeRegistry.GetIdByType(source.Value.FindValueType())));
            }
            var result = BlobVariantExtension.Read<float, MockNodeBlob, MockBlackboard>(ref variant.Value, 0, ref _nodeBlob, ref _bb);
            Assert.That(result, Is.EqualTo((4.1f + 5) * 2 + 10));
        }
    }
}