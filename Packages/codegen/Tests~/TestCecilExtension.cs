using System;
using System.Linq;
using EntitiesBT.CodeGen.Editor;
using JetBrains.Annotations;
using Mono.Cecil;
using NUnit.Framework;

namespace EntitiesBT.CodeGen.Tests
{
    public class TestCecilExtension
    {
        class TestAttr : Attribute
        {
            public enum TestEnum
            {
                One = 1, Two = 2
            }

            private int _privateValue;
            protected int _protectedValue;
            internal int InternalValue;
            public int PublicValue;
            public int PrivateValueProperty => _privateValue;
            public int ProtectedValueProperty => _protectedValue;
            public int PublicValueProperty { get; set; } = 100;

            public TestEnum Enum;

            public TestAttr(int privateValue, int protectedValue, int internalValue, int publicValue = 5, TestEnum @enum = TestEnum.Two)
            {
                _privateValue = privateValue;
                _protectedValue = protectedValue;
                InternalValue = internalValue;
                PublicValue = publicValue;
                Enum = @enum;
            }
        }

        class SubTestAttr : TestAttr
        {
            public SubTestAttr() : base(5, 6, 7, 8)
            {
            }
        }

        [TestAttr(1, 2, 3, 4)] class ClassWithAttribute {}
        [SubTestAttr] class ClassWithSubAttribute {}
        class ClassWithoutAttribute {}

        private AssemblyDefinition _assemblyDefinition;

        [SetUp]
        public void SetUp()
        {
            _assemblyDefinition = AssemblyDefinition.ReadAssembly(GetType().Assembly.Location, new ReaderParameters
            {
                AssemblyResolver = new DefaultAssemblyResolver()
            });
        }

        [Test]
        public void should_check_if_type_has_certain_attribute()
        {
            Assert.IsTrue(_assemblyDefinition.FindTypeDefinition<ClassWithAttribute>().HasAttribute<TestAttr>());
            Assert.IsTrue(_assemblyDefinition.FindTypeDefinition<ClassWithSubAttribute>().HasAttribute<TestAttr>());
            Assert.IsFalse(_assemblyDefinition.FindTypeDefinition<ClassWithoutAttribute>().HasAttribute<TestAttr>());
        }

        [Test]
        public void should_convert_cecil_attribute_into_reflection_one()
        {
            var cecilAttribute = _assemblyDefinition
                .FindTypeDefinition<ClassWithAttribute>()
                .CustomAttributes
                .Single(att => att.AttributeType.Resolve().ToReflectionType() == typeof(TestAttr))
            ;
            var attribute = cecilAttribute.ToAttribute<TestAttr>();
            Assert.AreEqual(1, attribute.PrivateValueProperty);
            Assert.AreEqual(2, attribute.ProtectedValueProperty);
            Assert.AreEqual(3, attribute.InternalValue);
            Assert.AreEqual(4, attribute.PublicValue);
            Assert.AreEqual(100, attribute.PublicValueProperty);
        }

        [Test]
        public void should_convert_cecil_sub_attribute_into_reflection_sub_one()
        {
            var cecilAttribute = _assemblyDefinition
                .FindTypeDefinition<ClassWithSubAttribute>()
                .CustomAttributes
                .Single(att => att.AttributeType.Resolve().ToReflectionType() == typeof(SubTestAttr))
            ;
            var attribute = cecilAttribute.ToAttribute<SubTestAttr>();
            Assert.AreEqual(5, attribute.PrivateValueProperty);
            Assert.AreEqual(6, attribute.ProtectedValueProperty);
            Assert.AreEqual(7, attribute.InternalValue);
            Assert.AreEqual(8, attribute.PublicValue);
            Assert.AreEqual(100, attribute.PublicValueProperty);
        }

        [Test]
        public void should_throw_if_attribute_type_is_not_match()
        {
            var cecilAttribute = _assemblyDefinition
                .FindTypeDefinition<ClassWithSubAttribute>()
                .CustomAttributes
                .Single(att => att.AttributeType.Resolve().ToReflectionType() == typeof(SubTestAttr))
            ;
            Assert.Throws<ArgumentException>(() => cecilAttribute.ToAttribute<TestAttr>());
        }

        [Test]
        public void should_get_attribute_from_type_definition()
        {
            var attribute = _assemblyDefinition
                .FindTypeDefinition<ClassWithAttribute>()
                .GetAttribute<TestAttr>()
            ;
            Assert.AreEqual(1, attribute.PrivateValueProperty);
            Assert.AreEqual(2, attribute.ProtectedValueProperty);
            Assert.AreEqual(3, attribute.InternalValue);
            Assert.AreEqual(4, attribute.PublicValue);
            Assert.AreEqual(100, attribute.PublicValueProperty);
        }
    }
}