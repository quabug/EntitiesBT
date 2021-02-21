using System;
using System.Linq;
using System.Reflection;
using EntitiesBT.CodeGen.Editor;
using EntitiesBT.Core;
using Mono.Cecil;
using NUnit.Framework;
using Unity.Entities;

namespace EntitiesBT.CodeGen.Tests
{
    public class TestCodeGenForNode
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
        public void should_generate_accessor_attribute_by_function_call_of_method()
        {
            var tick = typeof(TestNode).GetMethod("Tick", BindingFlags.Instance | BindingFlags.Public);
            var tickRO = tick.GetCustomAttributes<ReadOnlyAttribute>();
            var tickRW = tick.GetCustomAttributes<ReadWriteAttribute>();

            Assert.AreEqual(4, tickRO.Count());
            // Assert.Contains(typeof(FooComponent), tickRO);
            // Assert.Contains(typeof(int), tickRO);
            // Assert.Contains(typeof(float), tickRO);
            // Assert.Contains(typeof(ulong), tickRO);

            Assert.AreEqual(2, tickRW.Count());
            // Assert.Contains(typeof(BarComponent), tickRW);
            // Assert.Contains(typeof(byte), tickRW);

            var reset = typeof(TestNode).GetMethod("Reset", BindingFlags.Instance | BindingFlags.Public);
            Assert.AreEqual(3, reset.GetCustomAttributes<ReadOnlyAttribute>().Count());
            Assert.AreEqual(3, reset.GetCustomAttributes<ReadWriteAttribute>().Count());
        }

        struct FooComponent : IComponentData {}
        struct BarComponent : IComponentData {}

        class TestNode : INodeData
        {
            public NodeState Tick<TNodeBlob, TBlackboard>(int index, ref TNodeBlob blob, ref TBlackboard bb) where TNodeBlob : struct, INodeBlob where TBlackboard : struct, IBlackboard
            {
                bb.GetData<FooComponent>();
                bb.GetDataPtrRW(typeof(byte));
                ReadOnlyCalli(typeof(int));
                ReadOnlyCallvirt<float>();
                ReadWriteBar();
                StaticCall(0);
                return NodeState.Failure;
            }

            public void Reset<TNodeBlob, TBlackboard>(int index, ref TNodeBlob blob, ref TBlackboard bb) where TNodeBlob : struct, INodeBlob where TBlackboard : struct, IBlackboard
            {
                ReadOnlyDelgate readOnlyDelegate = ReadOnlyCalli;
                readOnlyDelegate(typeof(FooComponent));
                ComplexCall<ulong, float>(0);
            }

            delegate void ReadOnlyDelgate([ReadOnly] Type type);

            void ReadOnlyCalli([ReadOnly] Type type) {}
            public virtual void ReadOnlyCallvirt<[ReadOnly] T>() {}

            [ReadWrite(typeof(BarComponent))]
            private ref BarComponent ReadWriteBar() => throw new NotImplementedException();

            [ReadWrite(typeof(long)), ReadOnly(typeof(ushort), typeof(uint))]
            public void ComplexCall<[ReadOnly] T, [ReadWrite] U>([ReadWrite] double d) {}

            static void StaticCall([ReadOnly] ulong _) {}
        }
    }
}