using System;
using Blob;
using EntitiesBT.Variant;
using NUnit.Framework;
using Nuwa.Blob;
using Unity.Entities;

namespace EntitiesBT.Test
{
    public class TestVariant
    {
        [Test]
        public void should_find_value_type_of_variant()
        {
            var variant = new LocalVariant.Reader<int>();
            Assert.That(variant.FindValueType(), Is.EqualTo(typeof(int)));
        }

        class InvalidVariant : IVariant
        {
            public void Allocate(BlobVariantStream stream)
            {
            }

            public object PreviewValue => throw new NotImplementedException();
        }

        [Test]
        public void should_get_null_as_value_type_from_invalid_variant()
        {
            var variant = new InvalidVariant();
            Assert.That(variant.FindValueType(), Is.EqualTo(null));
        }
    }

    public class TestComponentVariant
    {
        private BlobMemoryStream _blobStream;
        private BlobVariantStream _variantStream;

        [SetUp]
        public void SetUp()
        {
            _blobStream = new BlobMemoryStream();
            _variantStream = new BlobVariantStream(_blobStream);
        }
        
        struct ComponentVariableData : IComponentData
        {
            public float FloatValue;
            public int IntValue;
            public long LongValue;
        }
        
        [Test]
        public void should_find_value_type_of_variant()
        {
            var variant = new ComponentVariant.ReaderAndWriter<int>();
            var type = typeof(ComponentVariableData);
            variant.ComponentValueName = $"{type.FullName}.{nameof(ComponentVariableData.IntValue)}";
            variant.Allocate(_variantStream);
            var blob = new ManagedBlobAssetReference<BlobVariant>(_blobStream.ToArray());
            Assert.That(blob.Value.VariantId, Is.EqualTo(Guid.Parse(ComponentVariant.GUID).GetHashCode()));
            Assert.That(blob.Value.MetaDataOffsetPtr, Is.EqualTo(4));
            ref var data = ref blob.Value.As<ComponentVariant.DynamicComponentData>();
            Assert.That(data.StableHash, Is.EqualTo(TypeHash.CalculateStableTypeHash(type)));
        }
        
        struct Variants
        {
            public BlobVariantRO<int> IntRO;
            public BlobVariantRW<float> FloatRW;
        }
        //
        // [Test]
        // public void should_find_value_type_of_variant_()
        // {
        //     var type = typeof(ComponentVariableData);
        //     var intRO = new ComponentVariant.Reader<int>();
        //     intRO.ComponentValueName = $"{type.FullName}.{nameof(ComponentVariableData.IntValue)}";
        //     var floatRW = new ComponentVariant.ReaderAndWriter<float>();
        //     floatRW.ComponentValueName = $"{type.FullName}.{nameof(ComponentVariableData.FloatValue)}";
        //     var builder = new StructBuilder<Variants>();
        //     builder.SetBuilder(ref builder.Value.IntRO, new VariantBuilder<BlobVariantRO<int>>(intRO));
        //     builder.SetBuilder(ref builder.Value.FloatRW, new VariantBuilder<BlobVariantRW<float>>(floatRW));
        //     builder.Build(_blobStream);
        //     var blob = new ManagedBlobAssetReference<Variants>(_blobStream.ToArray());
        //     
        //     Assert.That(blob.Value.IntRO.Value.VariantId, Is.EqualTo(Guid.Parse(ComponentVariant.GUID).GetHashCode()));
        //     Assert.That(blob.Value.IntRO.Value.MetaDataOffsetPtr, Is.EqualTo(24));
        //     ref var intVariantData = ref blob.Value.IntRO.Value.As<ComponentVariant.DynamicComponentData>();
        //     Assert.That(intVariantData.StableHash, Is.EqualTo(TypeHash.CalculateStableTypeHash(type)));
        //     
        //     // Assert.That(blob.Value.FloatRW.Reader.Value.VariantId, Is.EqualTo(Guid.Parse(ComponentVariant.GUID).GetHashCode()));
        //     // Assert.That(blob.Value.FloatRW.Value.MetaDataOffsetPtr, Is.EqualTo(16));
        //     // ref var intVariantData = ref blob.Value.FloatRW.Value.As<ComponentVariant.DynamicComponentData>();
        //     // Assert.That(intVariantData.StableHash, Is.EqualTo(TypeHash.CalculateStableTypeHash(type)));
        // }
    }
}