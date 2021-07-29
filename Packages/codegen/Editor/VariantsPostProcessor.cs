using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using EntitiesBT.Components;
using EntitiesBT.Core;
using EntitiesBT.Variant;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using Unity.CompilationPipeline.Common.ILPostProcessing;
using Unity.Entities;

namespace EntitiesBT.CodeGen.Editor
{
    public class VariantsPostProcessor : ILPostProcessor
    {
        private const string _overrideAssemblyCSharp = "EntitiesBT.CodeGen";
        private const string _serializedClassName = "Serializable";

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
                var isVariantsGenerated = GenerateVariants();
                var isSerializableNodeDataGenerated = GenerateSerializedClass();
                if (!isVariantsGenerated && !isSerializableNodeDataGenerated)
                    return new ILPostProcessResult(null, logger.Messages);

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

            bool GenerateVariants()
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
                    .Select(valueType => module.ImportReference(valueType.Resolve()))
                    .ToArray()
                ;

                var ro = GenerateVariants(typeof(IVariantReader<>), valueTypes, typeTree);
                var wo = GenerateVariants(typeof(IVariantWriter<>), valueTypes, typeTree);
                var rw = GenerateVariants(typeof(IVariantReaderAndWriter<>), valueTypes, typeTree);

                return ro || wo || rw;

                bool GenerateVariants(Type interfaceType, IReadOnlyList<TypeReference> valueTypes, TypeTree typeTree)
                {
                    var @interface = module.ImportReference(interfaceType);
                    if (!typeTree.HasBaseType(@interface.Resolve())) return false;

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
                var wrapper = new TypeDefinition(
                    "EntitiesBT.Node.CodeGen",
                    "<Serializable>",
                    TypeAttributes.Class | TypeAttributes.Sealed | TypeAttributes.Abstract | TypeAttributes.Public | TypeAttributes.BeforeFieldInit
                );
                wrapper.BaseType = module.ImportReference(typeof(object));
                module.Types.Add(wrapper);
                foreach (var node in nodes)
                {
                    if (node.NestedTypes.All(type => type.Name != _serializedClassName &&
                                                     !typeof(ISerializableNodeData).IsAssignableFrom(type))
                    )
                    {
                        modified = true;
                        var type = CreateSerializedClass(module.ImportReference(node));
                        wrapper.NestedTypes.Add(type);
                    }
                }
                return modified;
            }

            TypeDefinition CreateSerializedClass(TypeReference node)
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
                var serializable = baseType.GenerateDerivedClass(new[] {node}, node.Name, module);

                //    .method family hidebysig virtual instance void
                //      Build(
                //        valuetype EntitiesBT.Nodes.DelayTimerNode& data,
                //        valuetype [Unity.Entities]Unity.Entities.BlobBuilder builder,
                //        class EntitiesBT.Core.INodeDataBuilder self,
                //        class EntitiesBT.Core.ITreeNode`1<class EntitiesBT.Core.INodeDataBuilder>[] tree
                //      ) cil managed
                //    {
                var method = new MethodDefinition(
                    "Build",
                    MethodAttributes.Virtual | MethodAttributes.Family | MethodAttributes.HideBySig,
                    module.ImportReference(typeof(void))
                );
                serializable.Methods.Add(method);
                var data = new ParameterDefinition("data", ParameterAttributes.None, node.MakeByReferenceType());
                var builder = new ParameterDefinition("builder", ParameterAttributes.None, module.ImportReference(typeof(BlobBuilder)));
                var self = new ParameterDefinition("self", ParameterAttributes.None, module.ImportReference(typeof(INodeDataBuilder)));
                var tree = new ParameterDefinition("tree", ParameterAttributes.None, module.ImportReference(typeof(ITreeNode<INodeDataBuilder>[])));
                method.Parameters.Add(data);
                method.Parameters.Add(builder);
                method.Parameters.Add(self);
                method.Parameters.Add(tree);

                var il = method.Body.Instructions;
                //      .maxstack 8
                //
                //      // [30 13 - 30 14]
                //      IL_0000: nop
                //
                //      // [31 17 - 31 87]
                //      IL_0001: ldarg.0      // this
                //      IL_0002: ldfld        class EntitiesBT.Variant.SerializedVariantRW`1<float32> EntitiesBT.Nodes.DelayTimerNode/Serialized::TimerSeconds
                //      IL_0007: ldarga.s     builder
                //      IL_0009: ldarg.1      // data
                //      IL_000a: ldflda       valuetype EntitiesBT.Variant.BlobVariantRW`1<float32> EntitiesBT.Nodes.DelayTimerNode::TimerSeconds
                //      IL_000f: ldarg.3      // self
                //      IL_0010: ldarg.s      tree
                //      IL_0012: call         native int EntitiesBT.Variant.Utilities::Allocate<float32>(class EntitiesBT.Variant.ISerializedVariantRW`1<!!0/*float32*/>, valuetype [Unity.Entities]Unity.Entities.BlobBuilder&, valuetype EntitiesBT.Variant.BlobVariantRW`1<!!0/*float32*/>&, class EntitiesBT.Core.INodeDataBuilder, class EntitiesBT.Core.ITreeNode`1<class EntitiesBT.Core.INodeDataBuilder>[])
                //      IL_0017: pop
                //
                //      // [32 13 - 32 14]
                //      IL_0018: ret
                //
                //    } // end of method Serialized::Build
                //  } // end of class Serialized

                foreach (var field in node.Resolve().Fields.Where(f => f.IsPublic).Select(f => module.ImportReference(f)))
                {
                    var fieldType = module.ImportReference(field.FieldType);
                    if (fieldType.IsTypeEqual(module.ImportReference(typeof(BlobString))))
                    {
                        var call = module.ImportMethod(typeof(BlobStringExtensions), nameof(BlobStringExtensions.AllocateString));
                        CreateBlobType(field, module.ImportReference(typeof(string)), call);
                    }
                    else if (fieldType.IsTypeEqual(module.ImportReference(typeof(BlobArray<>))))
                    {
                        var call = module.ImportMethod(typeof(BlobBuilderExtensions), nameof(BlobBuilderExtensions.AllocateArray));
                        var valueType = ((GenericInstanceType) fieldType).GenericArguments[0];
                        var genericCall = call.MakeGenericInstanceMethod(valueType);
                        CreateBlobType(field, valueType.MakeArrayType(), genericCall);
                    }
                    else if (fieldType.IsTypeEqual(module.ImportReference(typeof(BlobVariantRO<>))))
                    {
                        var call = module.ImportMethod(typeof(Variant.Utilities), nameof(Variant.Utilities.AllocateRO));
                        var variantType = module.ImportReference(typeof(SerializedVariantRO<>));
                        CreateBlobVariantType(field, (GenericInstanceType)fieldType, variantType, call);
                    }
                    else if (fieldType.IsTypeEqual(module.ImportReference(typeof(BlobVariantWO<>))))
                    {
                        var call = module.ImportMethod(typeof(Variant.Utilities), nameof(Variant.Utilities.AllocateWO));
                        var variantType = module.ImportReference(typeof(SerializedVariantWO<>));
                        CreateBlobVariantType(field, (GenericInstanceType)fieldType, variantType, call);
                    }
                    else if (fieldType.IsTypeEqual(module.ImportReference(typeof(BlobVariantRW<>))))
                    {
                        var call = module.ImportMethod(typeof(Variant.Utilities), nameof(Variant.Utilities.AllocateRW));
                        var variantType = module.ImportReference(typeof(SerializedVariantRW<>));
                        CreateBlobVariantType(field, (GenericInstanceType)fieldType, variantType, call);
                    }
                    else
                    {
                        CreateDefaultType(field);
                    }
                }
                il.Add(Instruction.Create(OpCodes.Ret));
                return serializable;

                // .field public class EntitiesBT.Variant.SerializedVariantRW`1<float32> TimerSeconds

                void CreateDefaultType(FieldReference blobField)
                {
                    var serializableField = new FieldDefinition(blobField.Name, FieldAttributes.Public, blobField.FieldType);
                    serializable.Fields.Add(serializableField);
                    // IL_0018: ldarg.1      // data
                    il.Add(Instruction.Create(OpCodes.Ldarg_1));
                    // IL_0019: ldarg.0      // this
                    il.Add(Instruction.Create(OpCodes.Ldarg_0));
                    // IL_001a: ldfld        int32 EntitiesBT.Nodes.DelayTimerNode/Serializable::A
                    il.Add(Instruction.Create(OpCodes.Ldfld, serializableField));
                    // IL_001f: stfld        int32 EntitiesBT.Nodes.DelayTimerNode::A
                    il.Add(Instruction.Create(OpCodes.Stfld, blobField));
                }

                void CreateBlobType(FieldReference blobField, TypeReference serializableType, MethodReference allocateMethod)
                {
                    var serializableField = new FieldDefinition(blobField.Name, FieldAttributes.Public, serializableType);
                    serializable.Fields.Add(serializableField);
                    // IL_002f: ldarga.s     builder
                    il.Add(Instruction.Create(OpCodes.Ldarga_S, builder));
                    // IL_0031: ldarg.1      // data
                    il.Add(Instruction.Create(OpCodes.Ldarg_1));
                    // IL_0032: ldflda       valuetype [Unity.Entities]Unity.Entities.BlobString EntitiesBT.Nodes.DelayTimerNode::String
                    il.Add(Instruction.Create(OpCodes.Ldflda, blobField));
                    // IL_0037: ldarg.0      // this
                    il.Add(Instruction.Create(OpCodes.Ldarg_0));
                    // IL_0038: ldfld        string EntitiesBT.Nodes.DelayTimerNode/Serializable::String
                    il.Add(Instruction.Create(OpCodes.Ldfld, serializableField));
                    // IL_003d: call         void [Unity.Entities]Unity.Entities.BlobStringExtensions::AllocateString(valuetype [Unity.Entities]Unity.Entities.BlobBuilder&, valuetype [Unity.Entities]Unity.Entities.BlobString&, string)
                    il.Add(Instruction.Create(OpCodes.Call, allocateMethod));
                    // IL_0042: nop
                }

                void CreateBlobVariantType(FieldReference blobVariantField, GenericInstanceType blobVariantFieldType, TypeReference serializableType, MethodReference call)
                {
                    var valueType = blobVariantFieldType.GenericArguments[0];
                    var allocateMethod = call.MakeGenericInstanceMethod(valueType);
                    serializableType = serializableType.MakeGenericInstanceType(valueType);
                    var serializableField = new FieldDefinition(blobVariantField.Name, FieldAttributes.Public, serializableType);
                    serializable.Fields.Add(serializableField);

                    // IL_0018: ldarg.0      // this
                    il.Add(Instruction.Create(OpCodes.Ldarg_0));
                    // IL_0019: ldfld        class EntitiesBT.Variant.SerializedVariantRO`1<float32> EntitiesBT.Nodes.DelayTimerNode/Serializable::RO
                    il.Add(Instruction.Create(OpCodes.Ldfld, serializableField));
                    // IL_001e: ldarga.s     builder
                    il.Add(Instruction.Create(OpCodes.Ldarga_S, builder));
                    // IL_0020: ldarg.1      // data
                    il.Add(Instruction.Create(OpCodes.Ldarg_1));
                    // IL_0021: ldflda       valuetype EntitiesBT.Variant.BlobVariantRO`1<float32> EntitiesBT.Nodes.DelayTimerNode::RO
                    il.Add(Instruction.Create(OpCodes.Ldflda, blobVariantField));
                    // IL_0026: ldarg.3      // self
                    il.Add(Instruction.Create(OpCodes.Ldarg_3));
                    // IL_0027: ldarg.s      tree
                    il.Add(Instruction.Create(OpCodes.Ldarg_S, tree));
                    // IL_0029: call         native int EntitiesBT.Variant.Utilities::AllocateRO<float32>(class EntitiesBT.Variant.IVariantReader`1<!!0/*float32*/>, valuetype [Unity.Entities]Unity.Entities.BlobBuilder&, valuetype EntitiesBT.Variant.BlobVariantRO`1<!!0/*float32*/>&, class EntitiesBT.Core.INodeDataBuilder, class EntitiesBT.Core.ITreeNode`1<class EntitiesBT.Core.INodeDataBuilder>[])
                    il.Add(Instruction.Create(OpCodes.Call, allocateMethod));
                    // IL_002e: pop
                    il.Add(Instruction.Create(OpCodes.Pop));
                }
            }

        }
    }
}