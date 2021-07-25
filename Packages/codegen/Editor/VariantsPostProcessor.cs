using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using EntitiesBT.Variant;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using Unity.CompilationPipeline.Common.ILPostProcessing;

namespace EntitiesBT.CodeGen.Editor
{
    public class VariantsPostProcessor : ILPostProcessor
    {
        private const string _overrideAssemblyCSharp = "Variants.CodeGen";

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

        public override ILPostProcessResult Process(ICompiledAssembly compiledAssembly)
        {
            var sw = Stopwatch.StartNew();
            using var resolver = new PostProcessorAssemblyResolver(compiledAssembly.References);
            using var assembly = compiledAssembly.LoadAssembly(resolver);
            var referenceAssemblies = compiledAssembly.LoadLibraryAssemblies(resolver).ToArray();
            var logger = assembly.CreateLogger();
            var module = assembly.MainModule;

            try
            {

                var allTypes = referenceAssemblies.Append(assembly)
                    .Where(asm => !asm.Name.Name.StartsWith("Unity.") &&
                                  !asm.Name.Name.StartsWith("UnityEditor.") &&
                                  !asm.Name.Name.StartsWith("UnityEngine.")
                    )
                    .SelectMany(asm => asm.MainModule.GetAllTypes())
                    .Where(type => type.GetAttributesOf<VariantClassAttribute>().Any())
                    .SelectMany(type => type.NestedTypes)
                    .ToArray()
                ;
                var typeTree = new TypeTree(allTypes, logger);
                var valueTypes = referenceAssemblies.Append(assembly)
                    .SelectMany(asm => asm.GetAttributesOf<VariantValueTypeAttribute>())
                    .Select(attribute => (TypeReference) attribute.ConstructorArguments[0].Value)
                    .Select(valueType => module.ImportReference(valueType))
                    .ToArray()
                ;

                var ro = GenerateVariants(typeof(IVariantReader<>), valueTypes, typeTree);
                var wo = GenerateVariants(typeof(IVariantWriter<>), valueTypes, typeTree);
                var rw = GenerateVariants(typeof(IVariantReaderAndWriter<>), valueTypes, typeTree);

                if (!ro && !wo && !rw) return new ILPostProcessResult(null, logger.Messages);

                var pe = new MemoryStream();
                var pdb = new MemoryStream();
                var writerParameters = new WriterParameters
                {
                    SymbolWriterProvider = new PortablePdbWriterProvider(), SymbolStream = pdb, WriteSymbols = true
                };
                assembly.Write(pe, writerParameters);
                return new ILPostProcessResult(new InMemoryAssembly(pe.ToArray(), pdb.ToArray()), logger.Messages);
            }
            finally
            {
                foreach (var reference in referenceAssemblies) reference.Dispose();
                logger.Info($"process Variants.AssemblyCSharp ({sw.ElapsedMilliseconds}ms) on {assembly.Name.Name}({string.Join(",", compiledAssembly.References.Where(r => r.StartsWith("Library")))})");
            }

            bool GenerateVariants(Type interfaceType, IReadOnlyList<TypeReference> valueTypes, TypeTree typeTree)
            {
                var @interface = module.ImportReference(interfaceType);
                logger.Info($"process interface {@interface.Name}");

                var wrapper = new TypeDefinition(
                    "EntitiesBT.Variant.CodeGen",
                    $"<{@interface.Name.Split('`')[0]}>",
                    TypeAttributes.Class | TypeAttributes.Sealed | TypeAttributes.Abstract | TypeAttributes.Public | TypeAttributes.BeforeFieldInit
                );
                wrapper.BaseType = module.ImportReference(typeof(object));
                module.Types.Add(wrapper);

                var modified = false;

                foreach (var (valueType, variantType) in
                    from valueType in valueTypes
                    from variantType in typeTree.GetOrCreateAllDerivedReference(@interface.MakeGenericInstanceType(valueType))
                    select (valueType, module.ImportReference(variantType))
                )
                {
                    var className = $"{valueType.Name}{variantType.DeclaringType.Name}";
                    var variantDefinition = CreateVariantDefinition(variantType, className);
                    wrapper.NestedTypes.Add(variantDefinition);
                    modified = true;
                }

                return modified;
            }

            TypeDefinition CreateVariantDefinition(TypeReference variantType, string className)
            {
                var genericArguments = variantType.IsGenericInstance
                    ? ((GenericInstanceType) variantType).GenericArguments
                    : (IEnumerable<TypeReference>)Array.Empty<TypeReference>()
                ;
                if (genericArguments.All(arg => !arg.IsGenericParameter))
                {
                    var classAttributes = TypeAttributes.Class | TypeAttributes.NestedPublic | TypeAttributes.BeforeFieldInit;
                    var generated = new TypeDefinition("", className, classAttributes);
                    generated.BaseType = variantType.HasGenericParameters ? variantType.MakeGenericInstanceType(genericArguments.ToArray()) : variantType;
                    var ctor = module.ImportReference(variantType.Resolve().GetConstructors().First(c => !c.HasParameters)).Resolve();
                    var ctorCall = new MethodReference(ctor.Name, module.ImportReference(ctor.ReturnType))
                    {
                        DeclaringType = generated.BaseType,
                        HasThis = ctor.HasThis,
                        ExplicitThis = ctor.ExplicitThis,
                        CallingConvention = ctor.CallingConvention,
                    };
                    generated.AddEmptyCtor(ctorCall);
                    return generated;
                }
                return null;
            }
        }
    }
}