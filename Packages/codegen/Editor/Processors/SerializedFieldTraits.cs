using System;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
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
                    var il = method.Body.Instructions;
                    // IL_0018: ldarg.1      // data
                    il.Add(Instruction.Create(OpCodes.Ldarg_1));
                    // IL_0019: ldarg.0      // this
                    il.Add(Instruction.Create(OpCodes.Ldarg_0));
                    // IL_001a: ldfld        int32 EntitiesBT.Nodes.DelayTimerNode/Serializable::A
                    il.Add(Instruction.Create(OpCodes.Ldfld, serializedField));
                    // IL_001f: stfld        int32 EntitiesBT.Nodes.DelayTimerNode::A
                    il.Add(Instruction.Create(OpCodes.Stfld, blobField));
                },
                generateLoad: null
            );
        }
    }

    internal class BlobStringFieldTrait : IFieldTrait
    {
        private readonly TypeReference _blobType;
        private readonly TypeReference _serializedType;
        private readonly MethodReference _allocate;

        public BlobStringFieldTrait(ModuleDefinition module)
        {
            _blobType = module.ImportReference(typeof(BlobString));
            _serializedType = module.ImportReference(typeof(string));
            _allocate = module.ImportMethod(typeof(BlobStringExtensions), nameof(BlobStringExtensions.AllocateString));
        }

        public IFieldTrait.Data TryMakeData(FieldReference blobField)
        {
            if (!_blobType.IsTypeEqual(blobField.FieldType)) return null;
            var serializedField = new FieldDefinition(blobField.Name, FieldAttributes.Public, _serializedType);
            return new IFieldTrait.Data
            (
                serializedField: serializedField,
                generateBuild: method => method.GenerateBlobTypeBuildIL(blobField, serializedField, _allocate),
                generateLoad: null
            );
        }
    }

    internal class BlobArrayFieldTrait : IFieldTrait
    {
        private readonly TypeReference _blobType;
        private readonly MethodReference _allocate;

        public BlobArrayFieldTrait(ModuleDefinition module)
        {
            _blobType = module.ImportReference(typeof(BlobArray<>));
            _allocate = module.ImportMethod(typeof(BlobStringExtensions), nameof(BlobStringExtensions.AllocateString));
        }

        public IFieldTrait.Data TryMakeData(FieldReference blobField)
        {
            if (!_blobType.IsTypeEqual(blobField.FieldType)) return null;
            var valueType = ((GenericInstanceType) blobField.FieldType).GenericArguments[0];
            var genericAllocate = _allocate.MakeGenericInstanceMethod(valueType);
            var serializedField = new FieldDefinition(blobField.Name, FieldAttributes.Public, valueType.MakeArrayType());
            return new IFieldTrait.Data
            (
                serializedField: serializedField,
                generateBuild: method => method.GenerateBlobTypeBuildIL(blobField, serializedField, genericAllocate),
                generateLoad: null
            );
        }
    }

    internal abstract class BlobVariantFieldTrait : IFieldTrait
    {
        protected abstract TypeReference BlobType { get; }
        protected abstract TypeReference SerializedType { get; }
        protected abstract MethodReference Allocate { get; }

        public IFieldTrait.Data TryMakeData(FieldReference blobField)
        {
            if (!BlobType.IsTypeEqual(blobField.FieldType)) return null;
            var valueType = ((GenericInstanceType) blobField.FieldType).GenericArguments[0];
            var genericAllocate = Allocate.MakeGenericInstanceMethod(valueType);
            var serializedField = new FieldDefinition(blobField.Name, FieldAttributes.Public, SerializedType.MakeGenericInstanceType(valueType));
            return new IFieldTrait.Data
            (
                serializedField: serializedField,
                generateBuild: method => method.GenerateBlobVariantTypeBuildIL(blobField, serializedField, genericAllocate),
                generateLoad: null
            );
        }
    }

    internal class BlobVariantROFieldTrait : BlobVariantFieldTrait
    {
        protected override TypeReference BlobType { get; }
        protected override TypeReference SerializedType { get; }
        protected override MethodReference Allocate { get; }

        public BlobVariantROFieldTrait(ModuleDefinition module)
        {
            BlobType = module.ImportReference(typeof(Variant.BlobVariantRO<>));
            SerializedType = module.ImportReference(typeof(Variant.SerializedVariantRO<>));
            Allocate = module.ImportMethod(typeof(Variant.Utilities), nameof(Variant.Utilities.AllocateRO));
        }
    }

    internal class BlobVariantWOFieldTrait : BlobVariantFieldTrait
    {
        protected override TypeReference BlobType { get; }
        protected override TypeReference SerializedType { get; }
        protected override MethodReference Allocate { get; }

        public BlobVariantWOFieldTrait(ModuleDefinition module)
        {
            BlobType = module.ImportReference(typeof(Variant.BlobVariantWO<>));
            SerializedType = module.ImportReference(typeof(Variant.SerializedVariantWO<>));
            Allocate = module.ImportMethod(typeof(Variant.Utilities), nameof(Variant.Utilities.AllocateWO));
        }
    }

    internal class BlobVariantRWFieldTrait : BlobVariantFieldTrait
    {
        protected override TypeReference BlobType { get; }
        protected override TypeReference SerializedType { get; }
        protected override MethodReference Allocate { get; }

        public BlobVariantRWFieldTrait(ModuleDefinition module)
        {
            BlobType = module.ImportReference(typeof(Variant.BlobVariantRW<>));
            SerializedType = module.ImportReference(typeof(Variant.SerializedVariantRW<>));
            Allocate = module.ImportMethod(typeof(Variant.Utilities), nameof(Variant.Utilities.AllocateRW));
        }
    }

    internal static class Extension
    {
        public static void GenerateBlobTypeBuildIL(this MethodDefinition build, FieldReference blobField, FieldDefinition serializedField, MethodReference allocateMethod)
        {
            var il = build.Body.Instructions;
            // IL_002f: ldarga.s     builder
            il.Add(Instruction.Create(OpCodes.Ldarga_S, build.Parameters[1]));
            // IL_0031: ldarg.1      // data
            il.Add(Instruction.Create(OpCodes.Ldarg_1));
            // IL_0032: ldflda       valuetype [Unity.Entities]Unity.Entities.BlobString EntitiesBT.Nodes.DelayTimerNode::String
            il.Add(Instruction.Create(OpCodes.Ldflda, blobField));
            // IL_0037: ldarg.0      // this
            il.Add(Instruction.Create(OpCodes.Ldarg_0));
            // IL_0038: ldfld        string EntitiesBT.Nodes.DelayTimerNode/Serializable::String
            il.Add(Instruction.Create(OpCodes.Ldfld, serializedField));
            // IL_003d: call         void [Unity.Entities]Unity.Entities.BlobStringExtensions::AllocateString(valuetype [Unity.Entities]Unity.Entities.BlobBuilder&, valuetype [Unity.Entities]Unity.Entities.BlobString&, string)
            il.Add(Instruction.Create(OpCodes.Call, allocateMethod));
            // IL_0042: nop
        }

        public static void GenerateBlobVariantTypeBuildIL(this MethodDefinition method, FieldReference blobField, FieldDefinition serializedField, MethodReference allocateMethod)
        {
            var il = method.Body.Instructions;
            // IL_0018: ldarg.0      // this
            il.Add(Instruction.Create(OpCodes.Ldarg_0));
            // IL_0019: ldfld        class EntitiesBT.Variant.SerializedVariantRO`1<float32> EntitiesBT.Nodes.DelayTimerNode/Serializable::RO
            il.Add(Instruction.Create(OpCodes.Ldfld, serializedField));
            // IL_001e: ldarga.s     builder
            il.Add(Instruction.Create(OpCodes.Ldarga_S, method.Parameters[1]));
            // IL_0020: ldarg.1      // data
            il.Add(Instruction.Create(OpCodes.Ldarg_1));
            // IL_0021: ldflda       valuetype EntitiesBT.Variant.BlobVariantRO`1<float32> EntitiesBT.Nodes.DelayTimerNode::RO
            il.Add(Instruction.Create(OpCodes.Ldflda, blobField));
            // IL_0026: ldarg.3      // self
            il.Add(Instruction.Create(OpCodes.Ldarg_3));
            // IL_0027: ldarg.s      tree
            il.Add(Instruction.Create(OpCodes.Ldarg_S, method.Parameters[3]));
            // IL_0029: call         native int EntitiesBT.Variant.Utilities::AllocateRO<float32>(class EntitiesBT.Variant.IVariantReader`1<!!0/*float32*/>, valuetype [Unity.Entities]Unity.Entities.BlobBuilder&, valuetype EntitiesBT.Variant.BlobVariantRO`1<!!0/*float32*/>&, class EntitiesBT.Core.INodeDataBuilder, class EntitiesBT.Core.ITreeNode`1<class EntitiesBT.Core.INodeDataBuilder>[])
            il.Add(Instruction.Create(OpCodes.Call, allocateMethod));
            // IL_002e: pop
            il.Add(Instruction.Create(OpCodes.Pop));
        }
    }
}