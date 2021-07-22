using System;
using System.Collections.Generic;
using System.Linq;
using EntitiesBT.Core;
using EntitiesBT.Variant;
using EntitiesBT.Variant.Expression;
using Moq;
using NUnit.Framework;
using Unity.Collections;
using Unity.Entities;

namespace EntitiesBT.Tests
{
    using TreeNodeBuilder = ITreeNode<INodeDataBuilder>;

    public class Tests
    {
        static class Binary<TLeft, TRight, TReturn>
                where TLeft : unmanaged
                where TRight : unmanaged
                where TReturn : unmanaged
        {
            delegate TReturn Read<TNodeBlob, TBlackboard>(int index, ref TNodeBlob blob, ref TBlackboard bb)
                where TNodeBlob : struct, INodeBlob
                where TBlackboard : struct, IBlackboard
            ;

            struct Blob
            {
                public int OperatorId;
                public BlobVariantReader<TLeft> Left;
                public BlobVariantReader<TRight> Right;

                public Read<TNodeBlob, TBlackboard> Execute<TNodeBlob, TBlackboard>(Func<TLeft, TRight, TReturn> func)
                    where TNodeBlob : struct, INodeBlob
                    where TBlackboard : struct, IBlackboard
                {
                    var self = this;
                    return (int index, ref TNodeBlob blob, ref TBlackboard bb) =>
                    {
                        var left = self.Left.Read(index, ref blob, ref bb);
                        var right = self.Right.Read(index, ref blob, ref bb);
                        return func(left, right);
                    };
                }
            }

            public class Reader : IVariantReader<TReturn>
            {
                public int VariantId;
                public int OperatorId;
                public IVariantReader<TLeft> Left;
                public IVariantReader<TRight> Right;

                public IntPtr Allocate(ref BlobBuilder builder, ref BlobVariant blobVariant, INodeDataBuilder self, ITreeNode<INodeDataBuilder>[] tree)
                {
                    blobVariant.VariantId = VariantId;
                    var opBlob = new Blob();
                    Left.Allocate(ref builder, ref opBlob.Left, self, tree);
                    Right.Allocate(ref builder, ref opBlob.Right, self, tree);
                    return builder.Allocate(ref blobVariant, opBlob);
                }
            }
        }

        private INodeDataBuilder _nodeDataBuilder;
        private BlobBuilder _blobBuilder;
        private INodeBlob _noddBlob;
        private IBlackboard _bb;

        // class SingleVariantNodeBuilder : INodeDataBuilder
        // {
        //     public IVariant Variant;
        //
        //     public int NodeId { get; }
        //     public BlobAssetReference Build(TreeNodeBuilder[] builders)
        //     {
        //         var blobVariant = new BlobVariant();
        //         var blobBuilder = new BlobBuilder();
        //         Variant.Allocate(ref blobBuilder, ref blobVariant, this, builders);
        //         var blob = blobBuilder.CreateBlobAssetReference<>()
        //     }
        //     public INodeDataBuilder Self => this;
        //     public IEnumerable<INodeDataBuilder> Children => Enumerable.Empty<INodeDataBuilder>();
        //
        //     public BlobAssetReference<BlobVariant> Build()
        //     {
        //
        //     }
        // }

        [SetUp]
        public void SetUp()
        {
            _blobBuilder = new BlobBuilder();
            _nodeDataBuilder = new Mock<INodeDataBuilder>().Object;
            _noddBlob = new Mock<INodeBlob>().Object;
            _bb = new Mock<IBlackboard>().Object;
        }

        [TearDown]
        public void TearDown()
        {
            _blobBuilder.Dispose();
        }

        [Test]
        public void Test()
        {
            var binaryOperator = new Binary<int, float, float>.Reader();
            binaryOperator.Left = new LocalVariant.Reader<int> {Value = 1};
            binaryOperator.Right = new LocalVariant.Reader<float> {Value = 2.5f};
            var value = new BlobVariant();
            binaryOperator.Allocate(ref _blobBuilder, ref value, _nodeDataBuilder, Array.Empty<TreeNodeBuilder>());
            var blobVariant = _blobBuilder.CreateBlobAssetReference<BlobVariant>(Allocator.Temp);
            // VariantRegisters<float>.GetReader(blobVariant.Value.VariantId)(ref blobVariant, 0, ref _noddBlob, ref _bb);
        }
    }
}