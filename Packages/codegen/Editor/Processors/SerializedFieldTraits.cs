using System;
using System.Linq;
using EntitiesBT.Components;
using EntitiesBT.Core;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using Mono.Collections.Generic;
using Unity.Entities;

namespace EntitiesBT.CodeGen.Editor
{
    internal interface IFieldTrait
    {
        public class Data
        {
            public FieldDefinition SerializedField { get; }
            public Action<MethodDefinition> GenerateBuild { get; }
            public Action<MethodDefinition> GenerateLoad { get; }

            public Data(FieldDefinition serializedField, Action<MethodDefinition> generateBuild, Action<MethodDefinition> generateLoad)
            {
                SerializedField = serializedField;
                GenerateBuild = generateBuild;
                GenerateLoad = generateLoad;
            }
        }

        Data TryMakeData(FieldReference blobField);
    }

    internal class DefaultFieldTrait : IFieldTrait
    {
        public IFieldTrait.Data TryMakeData(FieldReference blobField)
        {
            var serializedField = new FieldDefinition(blobField.Name, FieldAttributes.Public, blobField.FieldType);
            return new IFieldTrait.Data
            (
                serializedField: serializedField,
                generateBuild: method =>
                {
                    method.Body.Instructions.Insert(0,
                        // IL_0018: ldarg.1      // data
                        Instruction.Create(OpCodes.Ldarg_1),
                        // IL_0019: ldarg.0      // this
                        Instruction.Create(OpCodes.Ldarg_0),
                        // IL_001a: ldfld        int32 EntitiesBT.Nodes.DelayTimerNode/Serializable::A
                        Instruction.Create(OpCodes.Ldfld, serializedField),
                        // IL_001f: stfld        int32 EntitiesBT.Nodes.DelayTimerNode::A
                        Instruction.Create(OpCodes.Stfld, blobField)
                    );
                },
                generateLoad: method =>
                {
                    method.Body.Instructions.Insert(0,
                        // IL_0023: ldarg.0      // this
                        Instruction.Create(OpCodes.Ldarg_0),
                        // IL_0024: ldarg.1      // data
                        Instruction.Create(OpCodes.Ldarg_1),
                        // IL_0025: ldfld        int64 EntitiesBT.Sample.VariablesTestNode::Long
                        Instruction.Create(OpCodes.Ldfld, blobField),
                        // IL_002a: stfld        int64 EntitiesBT.Sample.VariablesTestNode/Serializable::Long
                        Instruction.Create(OpCodes.Stfld, serializedField)
                    );
                });
        }
    }

    internal class BlobStringFieldTrait : IFieldTrait
    {
        private readonly TypeReference _blobType;
        private readonly TypeReference _serializedType;
        private readonly MethodReference _allocate;
        private readonly MethodReference _load;

        public BlobStringFieldTrait(ModuleDefinition module)
        {
            _blobType = module.ImportReference(typeof(BlobString));
            _serializedType = module.ImportReference(typeof(string));
            _allocate = module.ImportMethod(typeof(BlobStringExtensions), nameof(BlobStringExtensions.AllocateString));
            _load = module.ImportMethod(typeof(BlobString), nameof(BlobString.ToString));
        }

        public IFieldTrait.Data TryMakeData(FieldReference blobField)
        {
            if (!_blobType.IsTypeEqual(blobField.FieldType)) return null;
            var serializedField = new FieldDefinition(blobField.Name, FieldAttributes.Public, _serializedType);
            return new IFieldTrait.Data
            (
                serializedField: serializedField,
                generateBuild: method => method.GenerateBlobTypeBuildIL(blobField, serializedField, _allocate),
                generateLoad: method => method.GenerateBlobTypeLoadIL(blobField, serializedField, _load)
            );
        }
    }

    internal class BlobArrayFieldTrait : IFieldTrait
    {
        private readonly TypeReference _blobType;
        private readonly MethodReference _allocate;
        private readonly MethodReference _load;

        public BlobArrayFieldTrait(ModuleDefinition module)
        {
            _blobType = module.ImportReference(typeof(BlobArray<>));
            _allocate = module.ImportMethod(typeof(BlobBuilderExtensions), nameof(BlobBuilderExtensions.AllocateArray));
            _load = module.ImportMethod(typeof(BlobArray<>), nameof(BlobArray<int>.ToArray));
        }

        public IFieldTrait.Data TryMakeData(FieldReference blobField)
        {
            if (!_blobType.IsTypeEqual(blobField.FieldType)) return null;
            var valueType = ((GenericInstanceType) blobField.FieldType).GenericArguments[0];
            var serializedField = new FieldDefinition(blobField.Name, FieldAttributes.Public, valueType.MakeArrayType());
            var genericAllocate = _allocate.MakeGenericInstanceMethod(valueType);
            var genericLoad = _load.MakeGenericHostMethod(_blobType.MakeGenericInstanceType(valueType));
            return new IFieldTrait.Data
            (
                serializedField: serializedField,
                generateBuild: method => method.GenerateBlobTypeBuildIL(blobField, serializedField, genericAllocate),
                generateLoad: method => method.GenerateBlobTypeLoadIL(blobField, serializedField, genericLoad)
            );
        }
    }

    internal abstract class BlobVariantFieldTrait : IFieldTrait
    {
        protected abstract TypeReference BlobType { get; }
        protected abstract TypeReference SerializedType { get; }
        protected abstract MethodReference Allocate { get; }
        protected abstract MethodReference IsNull { get; }
        private readonly VariableDefinition _boolVar;

        protected BlobVariantFieldTrait(ModuleDefinition module)
        {
            _boolVar = new VariableDefinition(module.ImportReference(typeof(bool)));
        }

        public IFieldTrait.Data TryMakeData(FieldReference blobField)
        {
            if (!BlobType.IsTypeEqual(blobField.FieldType)) return null;
            var valueType = ((GenericInstanceType) blobField.FieldType).GenericArguments[0];
            var genericAllocate = Allocate.MakeGenericInstanceMethod(valueType);
            var genericSerializedType = SerializedType.MakeGenericInstanceType(valueType);
            var serializedField = new FieldDefinition(blobField.Name, FieldAttributes.Public, genericSerializedType);
            var isNull = IsNull.MakeGenericHostMethod(genericSerializedType);
            return new IFieldTrait.Data
            (
                serializedField: serializedField,
                generateBuild: method =>
                {
                    var lastInstruction = method.Body.Instructions[0];
                    method.GenerateBlobVariantTypeBuildIL(blobField, serializedField, genericAllocate);
                    if (blobField.IsOptional())
                        method.GenerateOptionalCheck(_boolVar, serializedField, isNull, lastInstruction);
                },
                generateLoad: method => {} // TODO: implement
            );
        }
    }

    internal class BlobVariantROFieldTrait : BlobVariantFieldTrait
    {
        protected override TypeReference BlobType { get; }
        protected override TypeReference SerializedType { get; }
        protected override MethodReference Allocate { get; }
        protected override MethodReference IsNull { get; }

        public BlobVariantROFieldTrait(ModuleDefinition module) : base(module)
        {
            BlobType = module.ImportReference(typeof(Variant.BlobVariantRO<>));
            SerializedType = module.ImportReference(typeof(Variant.SerializedVariantRO<>));
            Allocate = module.ImportMethod(typeof(Variant.Utilities), nameof(Variant.Utilities.AllocateRO));
            IsNull = module.ImportMethod(typeof(Variant.SerializedVariantRO<>), nameof(Variant.SerializedVariantRO<int>.IsNull));
        }
    }

    internal class BlobVariantWOFieldTrait : BlobVariantFieldTrait
    {
        protected override TypeReference BlobType { get; }
        protected override TypeReference SerializedType { get; }
        protected override MethodReference Allocate { get; }
        protected override MethodReference IsNull { get; }

        public BlobVariantWOFieldTrait(ModuleDefinition module) : base(module)
        {
            BlobType = module.ImportReference(typeof(Variant.BlobVariantWO<>));
            SerializedType = module.ImportReference(typeof(Variant.SerializedVariantWO<>));
            Allocate = module.ImportMethod(typeof(Variant.Utilities), nameof(Variant.Utilities.AllocateWO));
            IsNull = module.ImportMethod(typeof(Variant.SerializedVariantWO<>), nameof(Variant.SerializedVariantWO<int>.IsNull));
        }
    }

    internal class BlobVariantRWFieldTrait : BlobVariantFieldTrait
    {
        protected override TypeReference BlobType { get; }
        protected override TypeReference SerializedType { get; }
        protected override MethodReference Allocate { get; }
        protected override MethodReference IsNull { get; }

        public BlobVariantRWFieldTrait(ModuleDefinition module) : base(module)
        {
            BlobType = module.ImportReference(typeof(Variant.BlobVariantRW<>));
            SerializedType = module.ImportReference(typeof(Variant.SerializedVariantRW<>));
            Allocate = module.ImportMethod(typeof(Variant.Utilities), nameof(Variant.Utilities.AllocateRW));
            IsNull = module.ImportMethod(typeof(Variant.SerializedVariantRW<>), nameof(Variant.SerializedVariantRW<int>.IsNull));
        }
    }

    internal static class Extension
    {
        public static void GenerateBlobTypeBuildIL(this MethodDefinition build, FieldReference blobField, FieldDefinition serializedField, MethodReference allocateMethod)
        {
            build.Body.Instructions.Insert(0,
                // IL_002f: ldarga.s     builder
                Instruction.Create(OpCodes.Ldarga_S, build.Parameters[1]),
                // IL_0031: ldarg.1      // data
                Instruction.Create(OpCodes.Ldarg_1),
                // IL_0032: ldflda       valuetype [Unity.Entities]Unity.Entities.BlobString EntitiesBT.Nodes.DelayTimerNode::String
                Instruction.Create(OpCodes.Ldflda, blobField),
                // IL_0037: ldarg.0      // this
                Instruction.Create(OpCodes.Ldarg_0),
                // IL_0038: ldfld        string EntitiesBT.Nodes.DelayTimerNode/Serializable::String
                Instruction.Create(OpCodes.Ldfld, serializedField),
                // IL_003d: call         void [Unity.Entities]Unity.Entities.BlobStringExtensions::AllocateString(valuetype [Unity.Entities]Unity.Entities.BlobBuilder&, valuetype [Unity.Entities]Unity.Entities.BlobString&, string)
                Instruction.Create(OpCodes.Call, allocateMethod)
                // IL_0042: nop
            );
        }

        public static void GenerateBlobTypeLoadIL(this MethodDefinition load, FieldReference blobField, FieldDefinition serializedField, MethodReference loadMethod)
        {
            load.Body.Instructions.Insert(0,
                //IL_0001: ldarg.0      // this
                Instruction.Create(OpCodes.Ldarg_0),
                // IL_0002: ldarg.1      // data
                Instruction.Create(OpCodes.Ldarg_1),
                // IL_0003: ldflda       valuetype [Unity.Entities]Unity.Entities.BlobString EntitiesBT.Sample.VariablesTestNode::String
                Instruction.Create(OpCodes.Ldflda, blobField),
                // IL_0008: call         instance string [Unity.Entities]Unity.Entities.BlobString::ToString()
                Instruction.Create(OpCodes.Call, loadMethod),
                // IL_000d: stfld        string EntitiesBT.Sample.VariablesTestNode/Serializable::String
                Instruction.Create(OpCodes.Stfld, serializedField)
            );
        }

        public static void GenerateBlobVariantTypeBuildIL(
            this MethodDefinition method,
            FieldReference blobField,
            FieldDefinition serializedField,
            MethodReference allocateMethod
        )
        {
            method.Body.Instructions.Insert(0,
                // IL_0018: ldarg.0      // this
                Instruction.Create(OpCodes.Ldarg_0),
                // IL_0019: ldfld        class EntitiesBT.Variant.SerializedVariantRO`1<float32> EntitiesBT.Nodes.DelayTimerNode/Serializable::RO
                Instruction.Create(OpCodes.Ldfld, serializedField),
                // IL_001e: ldarga.s     builder
                Instruction.Create(OpCodes.Ldarga_S, method.Parameters[1]),
                // IL_0020: ldarg.1      // data
                Instruction.Create(OpCodes.Ldarg_1),
                // IL_0021: ldflda       valuetype EntitiesBT.Variant.BlobVariantRO`1<float32> EntitiesBT.Nodes.DelayTimerNode::RO
                Instruction.Create(OpCodes.Ldflda, blobField),
                // IL_0026: ldarg.3      // self
                Instruction.Create(OpCodes.Ldarg_3),
                // IL_0027: ldarg.s      tree
                Instruction.Create(OpCodes.Ldarg_S, method.Parameters[3]),
                // IL_0029: call         native int EntitiesBT.Variant.Utilities::AllocateRO<float32>(class EntitiesBT.Variant.IVariantReader`1<!!0/*float32*/>, valuetype [Unity.Entities]Unity.Entities.BlobBuilder&, valuetype EntitiesBT.Variant.BlobVariantRO`1<!!0/*float32*/>&, class EntitiesBT.Core.INodeDataBuilder, class EntitiesBT.Core.ITreeNode`1<class EntitiesBT.Core.INodeDataBuilder>[])
                Instruction.Create(OpCodes.Call, allocateMethod),
                // IL_002e: pop
                Instruction.Create(OpCodes.Pop)
            );
        }

        public static void GenerateOptionalCheck(
            this MethodDefinition method,
            VariableDefinition boolVar,
            FieldDefinition serializedField,
            MethodReference isNull,
            Instruction jumpTo
        )
        {
            method.Body.Variables.Add(boolVar);
            method.Body.Instructions.Insert(0,
                // IL_0001: ldarg.0      // this
                Instruction.Create(OpCodes.Ldarg_0),
                // IL_0002: ldfld        class [EntitiesBT.Runtime]EntitiesBT.Variant.SerializedVariantRO`1<int64> EntitiesBT.Sample.VariablesTestNode/Serializable::LongReader
                Instruction.Create(OpCodes.Ldfld, serializedField),
                // IL_0007: callvirt     instance bool class [EntitiesBT.Runtime]EntitiesBT.Variant.SerializedVariantRO`1<int64>::IsNull()
                Instruction.Create(OpCodes.Callvirt, isNull),
                // IL_000c: ldc.i4.0
                Instruction.Create(OpCodes.Ldc_I4_0),
                // IL_000d: ceq
                Instruction.Create(OpCodes.Ceq),
                // IL_000f: stloc.0      // V_0
                Instruction.Create(OpCodes.Stloc, boolVar),
                // IL_0010: ldloc.0      // V_0
                Instruction.Create(OpCodes.Ldloc, boolVar),
                // IL_0011: brfalse.s    IL_002
                Instruction.Create(OpCodes.Brfalse_S, jumpTo)
            );
        }

        public static bool IsOptional(this FieldReference blobField)
        {
            return blobField.Resolve().GetAttributesOf<OptionalAttribute>().Any();
        }

        internal static void Insert(this Collection<Instruction> il, int index, params Instruction[] instructions)
        {
            foreach (var instruction in instructions.Reverse()) il.Insert(index, instruction);
        }
    }
}