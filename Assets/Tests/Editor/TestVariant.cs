using System;
using EntitiesBT.Variant;
using NUnit.Framework;

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
}