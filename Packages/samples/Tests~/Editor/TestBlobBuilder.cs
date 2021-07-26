using EntitiesBT.Components;
using EntitiesBT.Variant;
using NUnit.Framework;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;

namespace EntitiesBT.Test
{
    public class TestBlobBuilder
    {
        struct Data
        {
            public BlobString StringValue;
            public BlobArray<int> IntArrayValue;
            public BlobArray<BlobString> StringArrayValue;
            public int IntValue;
        }

        [Test]
        public void should_build_complex_data_into_blob()
        {
            var builder = new BlobBuilder(Allocator.Temp);
            try
            {
                ref var variant = ref builder.ConstructRoot<BlobVariant>();
                variant.VariantId = 1;
                ref var blobPtr = ref UnsafeUtility.As<int, BlobPtr<Data>>(ref variant.MetaDataOffsetPtr);
                ref var data = ref builder.Allocate(ref blobPtr);
                builder.AllocateString(ref data.StringValue, "abc");
                data.IntValue = 2;
                builder.AllocateArray(ref data.IntArrayValue, new[] { 7, 8, 9});
                var stringArray = builder.Allocate(ref data.StringArrayValue, 2);
                builder.AllocateString(ref stringArray[0], "123");
                builder.AllocateString(ref stringArray[1], "456");

                using var persistent = builder.CreateBlobAssetReference<BlobVariant>(Allocator.Persistent);
                var value = persistent.Value;
                ref var valueData = ref persistent.Value.As<Data>();

                Assert.That(value.VariantId, Is.EqualTo(1));
                Assert.That(valueData.IntValue, Is.EqualTo(2));
                Assert.That(valueData.StringValue.ToString(), Is.EqualTo("abc"));

                Assert.That(valueData.IntArrayValue.Length, Is.EqualTo(3));
                Assert.That(valueData.IntArrayValue.ToArray(), Is.EquivalentTo(new [] { 7, 8, 9 }));

                Assert.That(valueData.StringArrayValue.Length, Is.EqualTo(2));
                Assert.That(valueData.StringArrayValue[0].ToString(), Is.EqualTo("123"));
                Assert.That(valueData.StringArrayValue[1].ToString(), Is.EqualTo("456"));
            }
            finally
            {
                builder.Dispose();
            }
        }
    }
}