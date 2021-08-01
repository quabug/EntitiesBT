using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using EntitiesBT.Core;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using Unity.CompilationPipeline.Common.ILPostProcessing;
using Unity.Entities;

namespace EntitiesBT.CodeGen.Editor
{

    public class SerializableNodeDataILPostProcessor : ILPostProcessor
    {
        private const string _overrideAssemblyCSharp = "EntitiesBT.SerializableNodeData.CodeGen";

        public override ILPostProcessor GetInstance()
        {
            return this;
        }

        public override bool WillProcess(ICompiledAssembly compiledAssembly)
        {
            if (compiledAssembly.Name == _overrideAssemblyCSharp) return true;
            var overrideDll = $"{_overrideAssemblyCSharp}.dll";
            var hasOverride = compiledAssembly.References.Any(@ref => @ref.EndsWith(overrideDll));
            return compiledAssembly.Name == "Assembly-CSharp" && !hasOverride;
        }

        private bool IsSerializableClass(TypeDefinition type)
        {
            return typeof(ISerializableNodeData).IsAssignableFrom(type);
        }

        public override ILPostProcessResult Process(ICompiledAssembly compiledAssembly)
        {
            var sw = Stopwatch.StartNew();
            using var resolver = new PostProcessorAssemblyResolver(compiledAssembly.References);
            using var assembly = compiledAssembly.LoadAssembly(resolver);
            var referenceAssemblies = compiledAssembly.LoadLibraryAssemblies(resolver).ToArray();
            var logger = assembly.CreateLogger();
            var module = assembly.MainModule;

            IReadOnlyList<IFieldTrait> fieldTraits = new IFieldTrait[]
            {
                new BlobStringFieldTrait(module),
                new BlobArrayFieldTrait(module),
                new BlobVariantROFieldTrait(module),
                new BlobVariantWOFieldTrait(module),
                new BlobVariantRWFieldTrait(module),
                new DefaultFieldTrait()
            };

            try
            {
                var isSerializableNodeDataGenerated = GenerateSerializedClass();
                return isSerializableNodeDataGenerated
                    ? assembly.Write(logger.Messages)
                    : new ILPostProcessResult(null, logger.Messages);
            }
            finally
            {
                foreach (var reference in referenceAssemblies) reference.Dispose();
                logger.Info($"process SerializableNodeData ({sw.ElapsedMilliseconds}ms) on {assembly.Name.Name}({string.Join(",", compiledAssembly.References.Where(r => r.StartsWith("Library")))})");
            }

            bool GenerateSerializedClass()
            {
                var nodes = referenceAssemblies.Append(assembly)
                    .Where(asm => !asm.Name.Name.StartsWith("Unity.") &&
                                  !asm.Name.Name.StartsWith("UnityEditor.") &&
                                  !asm.Name.Name.StartsWith("UnityEngine.")
                    )
                    .SelectMany(asm => asm.MainModule.GetAllTypes())
                    .Where(type => type.IsINodeDataStruct())
                    .ToArray()
                ;

                var modified = false;
                foreach (var node in nodes)
                {
                    if (node.NestedTypes.All(type => !IsSerializableClass(type)))
                    {
                        modified = true;
                        var type = CreateSerializableClass(module.ImportReference(node));
                        module.Types.Add(type);
                    }
                }

                return modified;
            }

            TypeDefinition CreateSerializableClass(TypeReference node)
            {
                //  .class nested public auto ansi beforefieldinit
                //    Serialized
                //      extends class EntitiesBT.Core.SerializedNodeData`1<valuetype EntitiesBT.Nodes.DelayTimerNode>
                //  {
                //
                //    .method public hidebysig specialname rtspecialname instance void
                //      .ctor() cil managed
                //    {
                //      .maxstack 8
                //
                //      IL_0000: ldarg.0      // this
                //      IL_0001: call         instance void class EntitiesBT.Core.SerializedNodeData`1<valuetype EntitiesBT.Nodes.DelayTimerNode>::.ctor()
                //      IL_0006: nop
                //      IL_0007: ret
                //
                //    } // end of method Serialized::.ctor
                var baseType = module.ImportReference(typeof(SerializableNodeData<>));
                var classAttributes = TypeAttributes.Class | TypeAttributes.Public | TypeAttributes.BeforeFieldInit;
                var serializable = new TypeDefinition(node.Namespace, $"<Serializable>_{node.Name}", classAttributes);
                serializable.BaseType = baseType.MakeGenericInstanceType(node);
                var ctor = module.ImportReference(baseType.Resolve().GetConstructors().First(c => !c.HasParameters)).Resolve();
                var ctorCall = new MethodReference(ctor.Name, module.ImportReference(ctor.ReturnType))
                {
                    DeclaringType = serializable.BaseType,
                    HasThis = ctor.HasThis,
                    ExplicitThis = ctor.ExplicitThis,
                    CallingConvention = ctor.CallingConvention,
                };

                serializable.AddEmptyCtor(ctorCall);

                //    .method family hidebysig virtual instance void
                //      Build(
                //        valuetype EntitiesBT.Nodes.DelayTimerNode& data,
                //        valuetype [Unity.Entities]Unity.Entities.BlobBuilder builder,
                //        class EntitiesBT.Core.INodeDataBuilder self,
                //        class EntitiesBT.Core.ITreeNode`1<class EntitiesBT.Core.INodeDataBuilder>[] tree
                //      ) cil managed
                //    {
                var buildMethod = CreateBuildMethod(node);
                buildMethod.Body.Instructions.Add(Instruction.Create(OpCodes.Ret));
                serializable.Methods.Add(buildMethod);

                var loadMethod = CreateLoadMethod(node);
                loadMethod.Body.Instructions.Add(Instruction.Create(OpCodes.Ret));
                serializable.Methods.Add(loadMethod);

                foreach (var field in node.Resolve().Fields.Where(f => f.IsPublic)
                    .Select(f => module.ImportReference(f)))
                {
                    var traitData = fieldTraits.Select(t => t.TryMakeData(field)).First(data => data != null);
                    serializable.Fields.Add(traitData.SerializedField);
                    traitData.GenerateBuild(buildMethod);
                    // traitData.GenerateLoad(loadMethod);
                }
                return serializable;
            }

            MethodDefinition CreateBuildMethod(TypeReference node)
            {
                var method = new MethodDefinition(
                    nameof(ISerializableNodeData.Build),
                    MethodAttributes.Virtual | MethodAttributes.Family | MethodAttributes.HideBySig,
                    module.ImportReference(typeof(void))
                );
                var data = new ParameterDefinition("data", ParameterAttributes.None, node.MakeByReferenceType());
                var builder = new ParameterDefinition("builder", ParameterAttributes.None, module.ImportReference(typeof(BlobBuilder)));
                var self = new ParameterDefinition("self", ParameterAttributes.None, module.ImportReference(typeof(INodeDataBuilder)));
                var tree = new ParameterDefinition("tree", ParameterAttributes.None, module.ImportReference(typeof(ITreeNode<INodeDataBuilder>[])));
                method.Parameters.Add(data);
                method.Parameters.Add(builder);
                method.Parameters.Add(self);
                method.Parameters.Add(tree);
                return method;
            }

            MethodDefinition CreateLoadMethod(TypeReference node)
            {
                var method = new MethodDefinition(
                    nameof(ISerializableNodeData.Load),
                    MethodAttributes.Virtual | MethodAttributes.Family | MethodAttributes.HideBySig,
                    module.ImportReference(typeof(void))
                );
                var data = new ParameterDefinition("data", ParameterAttributes.None, node.MakeByReferenceType());
                method.Parameters.Add(data);
                return method;
            }
        }


    }
}