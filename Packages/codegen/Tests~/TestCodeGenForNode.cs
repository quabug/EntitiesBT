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
        }

        [Test]
        public void should_avoid_generate_attributes_on_method_already_has_attributes()
        {
            var tick = typeof(TestManualNode).GetMethod("Tick", BindingFlags.Instance | BindingFlags.Public);
            var tickRO = tick.GetCustomAttributes<ReadOnlyAttribute>();
            var tickRW = tick.GetCustomAttributes<ReadWriteAttribute>();
            Assert.AreEqual(0, tickRO.Count());
            Assert.AreEqual(1, tickRW.Count());
            // Assert.AreEqual(ComponentType.ReadWrite<ulong>(), tickRW.Single());

            var reset = typeof(TestManualNode).GetMethod("Reset", BindingFlags.Instance | BindingFlags.Public);
            var resetRO = reset.GetCustomAttributes<ReadOnlyAttribute>();
            var resetRW = reset.GetCustomAttributes<ReadWriteAttribute>();
            Assert.AreEqual(1, resetRO.Count());
            Assert.AreEqual(0, resetRW.Count());
            // Assert.AreEqual(ComponentType.ReadOnly<bool>(), resetRO.Single());
        }

        class TestManualNode : INodeData
        {
            [ReadWrite(typeof(ulong))]
            public NodeState Tick<TNodeBlob, TBlackboard>(int index, ref TNodeBlob blob, ref TBlackboard bb) where TNodeBlob : struct, INodeBlob where TBlackboard : struct, IBlackboard
            {
                bb.GetData<int>();
                bb.GetDataPtrRW(typeof(float));
                return NodeState.Success;
            }

            [ReadOnly(typeof(bool))]
            public void Reset<TNodeBlob, TBlackboard>(int index, ref TNodeBlob blob, ref TBlackboard bb) where TNodeBlob : struct, INodeBlob where TBlackboard : struct, IBlackboard
            {
                bb.GetDataRef<float>();
            }
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