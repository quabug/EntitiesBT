using EntitiesBT.Core;
using EntitiesBT.Entities;
using EntitiesBT.Variant;
using EntitiesBT.Variant.Expression;
using NUnit.Framework;
using Unity.Collections;
using Unity.Entities;

namespace EntitiesBT.Test
{
    using TreeNodeBuilder = ITreeNode<INodeDataBuilder>;

    public class TestExpression
    {
        private BlobBuilder _builder;
        private NodeBlobRef _nodeBlob;
        private EntityBlackboard _bb;

        [SetUp]
        public void SetUp()
        {
            _builder = new BlobBuilder(Allocator.Temp);
            _nodeBlob = new NodeBlobRef();
            _bb = new EntityBlackboard();
        }

        [TearDown]
        public void TearDown()
        {
            _builder.Dispose();
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

            ref var buildVariant = ref _builder.ConstructRoot<BlobVariant>();
            expression.Allocate(ref _builder, ref buildVariant, null, null);
            using var variant = _builder.CreateBlobAssetReference<BlobVariant>(Allocator.Temp);
            var result = BlobVariantExtension.Read<float, NodeBlobRef, EntityBlackboard>(ref variant.Value, 0, ref _nodeBlob, ref _bb);
            Assert.That(result, Is.EqualTo((4.1f + 5) * 2 + 10));
        }
    }
}