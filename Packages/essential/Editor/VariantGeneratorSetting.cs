using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using EntitiesBT.Core;
using EntitiesBT.Variant;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using static UnityEditor.EditorUtility;
using static EntitiesBT.Variant.Utilities;

namespace EntitiesBT.Editor
{
    [CreateAssetMenu(fileName = "VariantGeneratorSetting", menuName = "EntitiesBT/VariantGeneratorSetting")]
    public class VariantGeneratorSetting : ScriptableObject
    {
        public string[] Types;
        public string Filename =  "VariantProperties";
        public string Namespace = $"{nameof(EntitiesBT)}.{nameof(Variant)}";
        public AssemblyDefinitionAsset Assembly;
        private ISet<Assembly> _referenceAssemblies;

        [ContextMenu("CreateScript")]
        public void CreateScript()
        {
            RecursiveFindReferences();
            var filePath = SaveFilePanel("Save Script", Directory, Filename, "cs");
            VariantGenerator.CreateScript(filePath, Namespace, Types, IsReferenceType);
        }

        [ContextMenu("CreateScript-InterfaceOnly")]
        public void CreateInterfaceScript()
        {
            RecursiveFindReferences();
            var filePath = SaveFilePanel("Save Script", Directory, $"{Filename}-Interface", "cs");
            VariantGenerator.CreateScriptInterfaceOnly(filePath, Namespace, Types);
        }

        [ContextMenu("CreateScript-ClassesOnly")]
        public void CreateClassesScript()
        {
            RecursiveFindReferences();
            var filePath = SaveFilePanel("Save Script", Directory, $"{Filename}-Classes", "cs");
            VariantGenerator.CreateScriptClassOnly(filePath, Namespace, Types, IsReferenceType);
        }

        private string Directory => Path.GetDirectoryName(AssetDatabase.GetAssetPath(this));

        private void RecursiveFindReferences()
        {
            _referenceAssemblies = Assembly.ToAssembly().FindReferenceAssemblies();
        }

        private bool IsReferenceType(Type type) => _referenceAssemblies.Contains(type.Assembly);
    }

    public static class VariantGenerator
    {
        public static void CreateScript(string filepath, string @namespace, string[] types, Predicate<Type> isReferenceType)
        {
            using (var writer = new StreamWriter(filepath))
            {
                writer.WriteLine(NamespaceBegin(@namespace));
                foreach (var type in _VALUE_TYPES.Value)
                {
                    if (types.Contains(type.FullName))
                    {
                        writer.CreateReaderVariants(type, isReferenceType);
                        writer.CreateWriterVariants(type, isReferenceType);
                        writer.CreateReaderAndWriterVariants(type, isReferenceType);
                        writer.CreateSerializedReaderAndWriterVariant(type);
                    }
                }
                writer.WriteLine(NamespaceEnd());
            }
            AssetDatabase.Refresh();
        }

        public static void CreateReaderVariants(this StreamWriter writer, Type valueType, Predicate<Type> isReferenceType = null, string suffix = "VariantReader")
        {
            writer.WriteLine(CreateInterface(valueType, typeof(IVariantReader<>), suffix));
            foreach (var propertyType in VARIANT_READER_TYPES.Value.Where(type => isReferenceType == null || isReferenceType(type)))
                writer.WriteLine(CreateClass(valueType, propertyType, suffix));
            writer.WriteLine();
        }

        public static void CreateWriterVariants(this StreamWriter writer, Type valueType, Predicate<Type> isReferenceType = null, string suffix = "VariantWriter")
        {
            writer.WriteLine(CreateInterface(valueType, typeof(IVariantWriter<>), suffix));
            foreach (var propertyType in VARIANT_WRITER_TYPES.Value.Where(type => isReferenceType == null || isReferenceType(type)))
                writer.WriteLine(CreateClass(valueType, propertyType, suffix));
            writer.WriteLine();
        }

        public static void CreateReaderAndWriterVariants(this StreamWriter writer, Type valueType, Predicate<Type> isReferenceType = null, string suffix = "VariantReaderAndWriter")
        {
            writer.WriteLine(CreateInterface(valueType, typeof(IVariantReaderAndWriter<>), suffix));
            foreach (var propertyType in VARIANT_READER_AND_WRITER_TYPES.Value.Where(type => isReferenceType == null || isReferenceType(type)))
                writer.WriteLine(CreateClass(valueType, propertyType, suffix));
            writer.WriteLine();
        }

        public static void CreateSerializedReaderAndWriterVariant(this StreamWriter writer
            , Type valueType
            , string readerAndWriterSuffix = "VariantReaderAndWriter"
            , string readerSuffix = "VariantReader"
            , string writerSuffix = "VariantWriter"
        )
        {
            writer.Write($@"
[System.Serializable]
public class {valueType.Name}SerializedReaderAndWriterVariant : EntitiesBT.Variant.ISerializedReaderAndWriter<{valueType.FullName}>
{{
    [UnityEngine.SerializeField]
    private bool _isLinked = true;
    public bool IsLinked => _isLinked;

    [UnityEngine.SerializeReference]
    [EntitiesBT.Attributes.HideIf(nameof(_isLinked), false)]
    [EntitiesBT.Attributes.SerializeReferenceButton]
    private {valueType.Name}{readerAndWriterSuffix} _readerAndWriter;
    public EntitiesBT.Variant.IVariantReaderAndWriter<{valueType.FullName}> ReaderAndWriter => _readerAndWriter;

    [UnityEngine.SerializeReference]
    [EntitiesBT.Attributes.HideIf(nameof(_isLinked))]
    [EntitiesBT.Attributes.SerializeReferenceButton]
    private {valueType.Name}{readerSuffix} _reader;
    public EntitiesBT.Variant.IVariantReader<{valueType.FullName}> Reader => _reader;

    [UnityEngine.SerializeReference]
    [EntitiesBT.Attributes.HideIf(nameof(_isLinked))]
    [EntitiesBT.Attributes.SerializeReferenceButton]
    private {valueType.Name}{writerSuffix} _writer;
    public EntitiesBT.Variant.IVariantWriter<{valueType.FullName}> Writer => _writer;
}}
");
        }

        public static void CreateScriptInterfaceOnly(string filepath, string @namespace, params string[] types)
        {
            using (var writer = new StreamWriter(filepath))
            {
                writer.WriteLine(NamespaceBegin(@namespace));
                foreach (var type in _VALUE_TYPES.Value)
                    if (types.Contains(type.FullName))
                    {
                        writer.WriteLine(CreateInterface(type, typeof(IVariantReader<>)));
                        writer.WriteLine(CreateInterface(type, typeof(IVariantWriter<>)));
                    }
                writer.WriteLine(NamespaceEnd());
            }
            AssetDatabase.Refresh();
        }

        public static void CreateScriptClassOnly(string filepath, string @namespace, string[] types, Predicate<Type> isReferenceType = null)
        {
            using (var writer = new StreamWriter(filepath))
            {
                writer.WriteLine(NamespaceBegin(@namespace));
                foreach (var type in _VALUE_TYPES.Value)
                {
                    if (types.Contains(type.FullName))
                    {
                        foreach (var propertyType in VARIANT_READER_TYPES.Value.Where(t => isReferenceType == null || isReferenceType(t)))
                            writer.WriteLine(CreateClass(type, propertyType));
                        foreach (var propertyType in VARIANT_WRITER_TYPES.Value.Where(t => isReferenceType == null || isReferenceType(t)))
                            writer.WriteLine(CreateClass(type, propertyType));
                    }
                }
                writer.WriteLine(NamespaceEnd());
            }
            AssetDatabase.Refresh();
        }

        private static string CreateInterface(Type type, Type variantType, string suffix = "Variant")
        {
            return $"public interface {type.Name}{suffix} : {variantType.FullName.ToCodeName()}<{type.FullName}> {{ }}";
        }

        private static string CreateClass(Type valueType, Type variantType, string suffix = "Variant", string classNameSuffix = null)
        {
            classNameSuffix ??= variantType.FullName.ToShortNameWithInnerClass();
            var className = $"{valueType.Name}{classNameSuffix}";
            var baseClassName = $"{variantType.FullName.ToCodeName()}<{valueType.FullName}>";
            var interfaceName = $"{valueType.Name}{suffix}";
            return $"public class {className} : {baseClassName}, {interfaceName} {{ }}";
        }

        public static string NamespaceBegin(string @namespace)
        {
            return $"namespace {@namespace}" + Environment.NewLine + "{" + Environment.NewLine;
        }

        public static string NamespaceEnd()
        {
            return Environment.NewLine + "}" + Environment.NewLine;
        }

        private static readonly Lazy<IEnumerable<Type>> _VALUE_TYPES = new Lazy<IEnumerable<Type>>(() =>
            Core.Utilities.ALL_TYPES.Value
                .Where(type => type != typeof(void))
                .Where(type => type.IsPrimitive || type.IsValueType && type.HasSerializableAttribute())
        );

        private static bool HasSerializableAttribute(this Type type)
        {
            return Attribute.GetCustomAttribute(type, typeof(SerializableAttribute)) != null;
        }
    }
}
