using NUnit.Framework;

namespace EntitiesBT.Test
{
    public class TestBT
    {
        [Test]
        public unsafe void TestNodeBlob()
        {
            // Debug.Log($"sizeof Composite: {sizeof(NodeDataA)}");
            // Debug.Log($"sizeof NodeA: {0}", (object) sizeof(NodeDataA)));
            // Debug.Log($"sizeof NodeB: {0}", (object) sizeof(NodeDataB)));
            //
            // int length = CompositeNodeData.Size + NodeDataA.Size + NodeDataB.Size;
            // using (BlobBuilder blobBuilder = new BlobBuilder(Allocator.Temp, 65536))
            // {
            //     ref NodeBlob local1 = ref blobBuilder.ConstructRoot<NodeBlob>();
            //     // ISSUE: cast to a reference type
            //     BlobBuilderArray<int> blobBuilderArray1 = blobBuilder.Allocate<int>((BlobArray<int> &) ref local1.Types ,  3);
            //     blobBuilderArray1[0] = 123;
            //     blobBuilderArray1[1] = 234;
            //     blobBuilderArray1[2] = 345;
            //     // ISSUE: cast to a reference type
            //     BlobBuilderArray<int> blobBuilderArray2 = blobBuilder.Allocate<int>((BlobArray<int> &) ref local1.EndIndices ,  3);
            //     blobBuilderArray2[0] = 3;
            //     blobBuilderArray2[1] = 2;
            //     blobBuilderArray2[2] = 3;
            //     // ISSUE: cast to a reference type
            //     BlobBuilderArray<int> blobBuilderArray3 = blobBuilder.Allocate<int>((BlobArray<int> &) ref local1.Offsets ,  3);
            //     // ISSUE: cast to a reference type
            //     byte* unsafePtr = (byte*) blobBuilder.Allocate<byte>((BlobArray<byte> &) ref local1.DataBlob , length).GetUnsafePtr();
            //     int num1 = 0;
            //     blobBuilderArray3[0] = num1;
            //     UnsafeUtilityEx.AsRef<CompositeNodeData>((void*) (unsafePtr + num1));
            //     int num2 = num1 + CompositeNodeData.Size;
            //     blobBuilderArray3[1] = num2;
            //     UnsafeUtilityEx.AsRef<NodeDataA>((void*) (unsafePtr + num2)).A = 111;
            //     int num3 = num2 + NodeDataA.Size;
            //     blobBuilderArray3[2] = num3;
            //     ref NodeDataB local2 = ref UnsafeUtilityEx.AsRef<NodeDataB>((void*) (unsafePtr + num3));
            //     local2.B = 222;
            //     local2.BB = 2222;
            //     BlobAssetReference<NodeBlob> blobAssetReference = blobBuilder.CreateBlobAssetReference<NodeBlob>(Allocator.Persistent);
            //     try
            //     {
            //         Assert.IsTrue(blobAssetReference.IsCreated);
            //         Assert.AreEqual((object) blobAssetReference.Value.DataBlob.Length, (object) length);
            //         Assert.AreEqual((object) ((NodeBlob) ref blobAssetReference.Value).get_Count(), (object) 3);
            //         Assert.AreEqual((object) blobAssetReference.Value.Types[0], (object) 123);
            //         Assert.AreEqual((object) blobAssetReference.Value.Types[1], (object) 234);
            //         Assert.AreEqual((object) blobAssetReference.Value.Types[2], (object) 345);
            //         Assert.AreEqual((object) blobAssetReference.Value.EndIndices[0], (object) 3);
            //         Assert.AreEqual((object) blobAssetReference.Value.EndIndices[1], (object) 2);
            //         Assert.AreEqual((object) blobAssetReference.Value.EndIndices[2], (object) 3);
            //         // ISSUE: cast to a reference type
            //         // ISSUE: explicit reference operation
            //         Assert.AreEqual((object) (^(NodeDataA &) ref ((NodeBlob) ref blobAssetReference.Value).GetNodeData<NodeDataA>(1)).A, (object) 111);
            //         // ISSUE: cast to a reference type
            //         ref NodeDataB local3 = (NodeDataB &) ref ((NodeBlob) ref blobAssetReference.Value ).GetNodeData<NodeDataB>(2);
            //         Assert.AreEqual((object) local3.B, (object) 222);
            //         Assert.AreEqual((object) local3.BB, (object) 2222);
            //     } finally
            //     {
            //         if (blobAssetReference.IsCreated) blobAssetReference.Dispose();
            //     }
            // }
        }

        [Test]
        public void TestSequence() { }
    }
}
