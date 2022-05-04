using System;
using Blob;
using EntitiesBT.Variant;
using NUnit.Framework;
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
        
        [Test]
        public void should_find_value_type_of_variant()
        {
            var variant = new ComponentVariant.ReaderAndWriter<int>();
            var type = Type.GetType("EntitiesBT.Sample.ComponentVariableData");
            variant.ComponentValueName = $"{type.FullName}.IntValue";
            variant.Allocate(_variantStream);
            var blob = new ManagedBlobAssetReference<BlobVariant>(_blobStream.ToArray());
            Assert.That(blob.Value.VariantId, Is.EqualTo(Guid.Parse(ComponentVariant.GUID).GetHashCode()));
            Assert.That(blob.Value.MetaDataOffsetPtr, Is.EqualTo(4));
            ref var data = ref blob.Value.As<ComponentVariant.DynamicComponentData>();
            Assert.That(data.StableHash, Is.EqualTo(TypeHash.CalculateStableTypeHash(type)));
        }
    }
}