using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    public class SerializableNodeDataILPostProcessor : ILPostProcessor
    {
        private const string _overrideAssemblyCSharp = "EntitiesBT.SerializableNodeData.CodeGen";
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
                var method = new MethodDefinition(
                    "Build",
                    MethodAttributes.Virtual | MethodAttributes.Family | MethodAttributes.HideBySig,
                    module.ImportReference(typeof(void))
                );
                serializable.Methods.Add(method);
                var data = new ParameterDefinition("data", ParameterAttributes.None, node.MakeByReferenceType());
                var builder = new ParameterDefinition("builder", ParameterAttributes.None,
                    module.ImportReference(typeof(BlobBuilder)));
                var self = new ParameterDefinition("self", ParameterAttributes.None,
                    module.ImportReference(typeof(INodeDataBuilder)));
                var tree = new ParameterDefinition("tree", ParameterAttributes.None,
                    module.ImportReference(typeof(ITreeNode<INodeDataBuilder>[])));
                method.Parameters.Add(data);
                method.Parameters.Add(builder);
                method.Parameters.Add(self);
                method.Parameters.Add(tree);

                var il = method.Body.Instructions;

                foreach (var field in node.Resolve().Fields.Where(f => f.IsPublic)
                    .Select(f => module.ImportReference(f)))
                {
                    var fieldType = module.ImportReference(field.FieldType);
                    if (fieldType.IsTypeEqual(module.ImportReference(typeof(BlobString))))
                    {
                        var call = module.ImportMethod(typeof(BlobStringExtensions),
                            nameof(BlobStringExtensions.AllocateString));
                        CreateBlobType(field, module.ImportReference(typeof(string)), call);
                    }
                    else if (fieldType.IsTypeEqual(module.ImportReference(typeof(BlobArray<>))))
                    {
                        var call = module.ImportMethod(typeof(BlobBuilderExtensions),
                            nameof(BlobBuilderExtensions.AllocateArray));
                        var valueType = ((GenericInstanceType) fieldType).GenericArguments[0];
                        var genericCall = call.MakeGenericInstanceMethod(valueType);
                        CreateBlobType(field, valueType.MakeArrayType(), genericCall);
                    }
                    else if (fieldType.IsTypeEqual(module.ImportReference(typeof(BlobVariantRO<>))))
                    {
                        var call = module.ImportMethod(typeof(Variant.Utilities), nameof(Variant.Utilities.AllocateRO));
                        var variantType = module.ImportReference(typeof(SerializedVariantRO<>));
                        CreateBlobVariantType(field, (GenericInstanceType) fieldType, variantType, call);
                    }
                    else if (fieldType.IsTypeEqual(module.ImportReference(typeof(BlobVariantWO<>))))
                    {
                        var call = module.ImportMethod(typeof(Variant.Utilities), nameof(Variant.Utilities.AllocateWO));
                        var variantType = module.ImportReference(typeof(SerializedVariantWO<>));
                        CreateBlobVariantType(field, (GenericInstanceType) fieldType, variantType, call);
                    }
                    else if (fieldType.IsTypeEqual(module.ImportReference(typeof(BlobVariantRW<>))))
                    {
                        var call = module.ImportMethod(typeof(Variant.Utilities), nameof(Variant.Utilities.AllocateRW));
                        var variantType = module.ImportReference(typeof(SerializedVariantRW<>));
                        CreateBlobVariantType(field, (GenericInstanceType) fieldType, variantType, call);
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
                    var serializableField =
                        new FieldDefinition(blobField.Name, FieldAttributes.Public, blobField.FieldType);
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

                void CreateBlobType(FieldReference blobField, TypeReference serializableType,
                    MethodReference allocateMethod)
                {
                    var serializableField =
                        new FieldDefinition(blobField.Name, FieldAttributes.Public, serializableType);
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

                void CreateBlobVariantType(FieldReference blobVariantField, GenericInstanceType blobVariantFieldType,
                    TypeReference serializableType, MethodReference call)
                {
                    var valueType = blobVariantFieldType.GenericArguments[0];
                    var allocateMethod = call.MakeGenericInstanceMethod(valueType);
                    serializableType = serializableType.MakeGenericInstanceType(valueType);
                    var serializableField =
                        new FieldDefinition(blobVariantField.Name, FieldAttributes.Public, serializableType);
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