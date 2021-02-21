using System;
using System.Linq;
using System.Reflection;
using EntitiesBT.CodeGen.Editor;
using EntitiesBT.Core;
using Mono.Cecil;
using NUnit.Framework;

namespace EntitiesBT.CodeGen.Tests
{
    public class TestGenerateAccessorAttributesFromMethod
    {
        private AssemblyDefinition _assembly;
        private TypeDefinition _selfType;
        private ModuleDefinition _module;

        [SetUp]
        public void SetUp()
        {
            _assembly = AssemblyDefinition.ReadAssembly(GetType().Assembly.Location, new ReaderParameters
            {
                AssemblyResolver = new PostProcessorAssemblyResolver(new []
                {
                    typeof(INodeData).Assembly.Location
                    , typeof(int).Assembly.Location
                    , typeof(Console).Assembly.Location
                })
                , ReflectionImporterProvider = new PostProcessorReflectionImporterProvider()
            });
            _selfType = _assembly.FindTypeDefinition(GetType());
            _module = _assembly.MainModule;
        }

        [Test]
        public void should_generate_attribute_of_blackboard_get_data()
        {
            var method = _selfType.GetMethod(nameof(CallBlackboardGetData));
            var attribute = method.GenerateAccessorAttributes().Single();
            CheckSingleCustomAttribute<ReadOnlyAttribute>(attribute, typeof(int));
        }
        void CallBlackboardGetData<T>(T bb) where T : IBlackboard => bb.GetData<int>();

        [Test]
        public void should_generate_attribute_of_blackboard_get_data_ref()
        {
            var method = _selfType.GetMethod(nameof(CallBlackboardGetDataRef));
            var attribute = method.GenerateAccessorAttributes().Single();
            CheckSingleCustomAttribute<ReadWriteAttribute>(attribute, typeof(float));
        }
        void CallBlackboardGetDataRef<T>(T bb) where T : IBlackboard => bb.GetDataRef<float>();

        [Test]
        public void should_generate_attribute_of_blackboard_get_data_ptr_ro()
        {
            var method = _selfType.GetMethod(nameof(CallBlackboardGetDataPtrRO));
            var attribute = method.GenerateAccessorAttributes().Single();
            CheckSingleCustomAttribute<ReadOnlyAttribute>(attribute, typeof(long));
        }
        void CallBlackboardGetDataPtrRO<T>(T bb) where T : IBlackboard => bb.GetDataPtrRO(typeof(long));

        [Test]
        public void should_generate_attribute_of_blackboard_get_data_ptr_rw()
        {
            var method = _selfType.GetMethod(nameof(CallBlackboardGetDataPtrRW));
            var attribute = method.GenerateAccessorAttributes().Single();
            CheckSingleCustomAttribute<ReadWriteAttribute>(attribute, typeof(short));
        }
        void CallBlackboardGetDataPtrRW<T>(T bb) where T : IBlackboard => bb.GetDataPtrRW(typeof(short));

        [Test]
        public void should_generate_attribute_from_method_attributes()
        {
            var method = _selfType.GetMethod(nameof(MethodAttributeCall));
            var attribute = method.GenerateAccessorAttributes().Single();
            CheckSingleCustomAttribute<ReadOnlyAttribute>(attribute, typeof(int));
        }
        void MethodAttributeCall() => MethodAttributes();
        [ReadOnly(typeof(int))] void MethodAttributes() {}

        [Test]
        public void should_generate_attribute_from_parameter_attributes()
        {
            var method = _selfType.GetMethod(nameof(ParameterAttributesCall));
            var attribute = method.GenerateAccessorAttributes().Single();
            CheckSingleCustomAttribute<ReadWriteAttribute>(attribute, typeof(float));
        }
        void ParameterAttributesCall() => ParameterAttributes(0);
        void ParameterAttributes([ReadWrite] float _) {}

        [Test, Ignore("order is not stable")]
        public void should_generate_attribute_of_complex_call()
        {
            var method = _selfType.GetMethod(nameof(CallComplex));
            var attributes = method.GenerateAccessorAttributes().ToArray();
            Assert.AreEqual(6, attributes.Length);
            CheckSingleCustomAttribute<ReadOnlyAttribute>(attributes[0], typeof(long));
            CheckSingleCustomAttribute<ReadWriteAttribute>(attributes[1], typeof(ulong));
            CheckSingleCustomAttribute<ReadOnlyAttribute>(attributes[2], typeof(int));
            CheckSingleCustomAttribute<ReadWriteAttribute>(attributes[3], typeof(uint));
            CheckSingleCustomAttribute<ReadWriteAttribute>(attributes[4], typeof(float));
            CheckSingleCustomAttribute<ReadOnlyAttribute>(attributes[5], typeof(double));
        }
        void CallComplex() => MultipleAttributes<long, ulong>(typeof(double));
        [ReadOnly(typeof(int)), ReadWrite(typeof(uint))]
        void MultipleAttributes<[ReadOnly] T, [ReadWrite] U>([ReadOnly] [ReadWrite(typeof(float))] Type t) {}

        [Test, Ignore("invalid throw???")]
        public void should_throw_if_not_supported()
        {
            var method = GetType().GetMethod(nameof(CallParameterAccessors), BindingFlags.Instance | BindingFlags.NonPublic).MakeGenericMethod(typeof(bool), typeof(byte));
            Assert.Catch<NotSupportedException>(() => _module.ImportReference(method).Resolve().GenerateAccessorAttributes());
        }
        private Type h = typeof(ulong);
        void CallParameterAccessors<T, U>()
        {
            // var f = 3;
            var c = typeof(U);
            var a = typeof(int);
            var b = 1;
            var e = 2;
            Call(a, b, c, typeof(T), e, GetDoubleType(0), typeof(long), h);

            Type GetDoubleType(int _) => typeof(double);
        }
        void Call(Type a, [ReadWrite] int b, [ReadOnly] Type c, Type d, [ReadOnly] long e, Type f, Type g, [ReadWrite] Type h) {}

        void CheckSingleCustomAttribute<T>(CustomAttribute attribute, params Type[] types) where T : ComponentAccessorAttribute
        {
            Assert.AreEqual(typeof(T), attribute.AttributeType.Resolve().ToReflectionType());
            Assert.AreEqual(1, attribute.ConstructorArguments.Count);
            var arguments = (CustomAttributeArgument[])attribute.ConstructorArguments.Single().Value;
            Assert.AreEqual(typeof(Type[]).FullName, attribute.ConstructorArguments.Single().Type.FullName);
            Assert.AreEqual(types.Length, arguments.Length);
            for (var i = 0; i < types.Length; i++)
            {
                var argument = arguments[i];
                Assert.AreEqual(typeof(Type), argument.Type.Resolve().ToReflectionType());
                Assert.AreEqual(types[i], ((TypeReference)argument.Value).Resolve().ToReflectionType());
            }
        }
    }
}