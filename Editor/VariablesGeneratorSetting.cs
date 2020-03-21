using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using EntitiesBT.Variable;
using Unity.Entities;
using UnityEditor;
using UnityEngine;

namespace EntitiesBT.Editor
{
    [CreateAssetMenu(fileName = "VariablesGeneratorSetting", menuName = "EntitiesBT/VariableGeneratorSetting")]
    public class VariablesGeneratorSetting : ScriptableObject
    {
        public string[] Types;
        public string Filename = "VariableProperties";
        public string Namespace = "EntitiesBT.Variable";

        [ContextMenu("CreateScript")]
        public void CreateScript()
        {
            var filePath = EditorUtility.SaveFilePanel("Save Script", Directory, Filename, "cs");
            VariableGenerator.CreateScript(filePath, Namespace, Types);
        }
        
        [ContextMenu("CreateScript-InterfaceOnly")]
        public void CreateInterfaceScript()
        {
            var filePath = EditorUtility.SaveFilePanel("Save Script", Directory, $"{Filename}-Interface", "cs");
            VariableGenerator.CreateScriptInterfaceOnly(filePath, Namespace, Types);
        }
        
        [ContextMenu("CreateScript-ClassesOnly")]
        public void CreateClassesScript()
        {
            var filePath = EditorUtility.SaveFilePanel("Save Script", Directory, $"{Filename}-Classes", "cs");
            VariableGenerator.CreateScriptClassOnly(filePath, Namespace, Types);
        }

        public string Directory => Path.GetDirectoryName(AssetDatabase.GetAssetPath(this));
    }
    
    public static class VariableGenerator
    {
        public static void CreateScript(string filepath, string @namespace, string[] types)
        {
            using (var writer = new StreamWriter(filepath))
            {
                writer.WriteLine(NamespaceBegin(@namespace));
                foreach (var type in _VALUE_TYPES.Value)
                {
                    if (types.Contains(type.FullName))
                    {
                        writer.WriteLine(CreateInterface(type));
                        foreach (var propertyType in _PROPERTY_TYPES.Value)
                            writer.WriteLine(CreateClass(type, propertyType));
                        writer.WriteLine();
                    }
                }
                writer.WriteLine(NamespaceEnd());
            }
            AssetDatabase.Refresh();
        }
        
        public static void CreateScriptInterfaceOnly(string filepath, string @namespace, string[] types)
        {
            using (var writer = new StreamWriter(filepath))
            {
                writer.WriteLine(NamespaceBegin(@namespace));
                foreach (var type in _VALUE_TYPES.Value)
                    if (types.Contains(type.FullName))
                        writer.WriteLine(CreateInterface(type));
                writer.WriteLine(NamespaceEnd());
            }
            AssetDatabase.Refresh();
        }
        
        public static void CreateScriptClassOnly(string filepath, string @namespace, string[] types)
        {
            using (var writer = new StreamWriter(filepath))
            {
                writer.WriteLine(NamespaceBegin(@namespace));
                foreach (var type in _VALUE_TYPES.Value)
                {
                    if (types.Contains(type.FullName))
                    {
                        foreach (var propertyType in _PROPERTY_TYPES.Value)
                            writer.WriteLine(CreateClass(type, propertyType));
                    }
                }
                writer.WriteLine(NamespaceEnd());
            }
            AssetDatabase.Refresh();
        }
        
        static string CreateInterface(Type type)
        {
            return $"public interface {type.Name}Property {{ void Allocate(ref Unity.Entities.BlobBuilder builder, ref EntitiesBT.Variable.BlobVariable<{type.FullName}> blobVariable, EntitiesBT.Core.INodeDataBuilder self, EntitiesBT.Core.ITreeNode<EntitiesBT.Core.INodeDataBuilder>[] tree); }}";
        }

        static string CreateClass(Type valueType, Type variablePropertyType)
        {
            return $"public class {valueType.Name}{variablePropertyType.Name.Split('`')[0]} : {variablePropertyType.FullName.Split('`')[0]}<{valueType.FullName}>, {valueType.Name}Property {{ }}";
        }

        static string NamespaceBegin(string @namespace)
        {
            return $"namespace {@namespace}" + Environment.NewLine + "{" + Environment.NewLine;
        }

        static string NamespaceEnd()
        {
            return Environment.NewLine + "}" + Environment.NewLine;
        }

        private static readonly Lazy<IEnumerable<Type>> _VALUE_TYPES = new Lazy<IEnumerable<Type>>(() => 
            AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes(), (assembly, type) => (assembly, type))
                .Where(t => t.type != typeof(void))
                .Where(t => t.type.IsPrimitive || (t.type.IsValueType && t.type.HasSerializableAttribute()))
                .Select(t => t.type))
            ;
        
        private static readonly Lazy<IEnumerable<Type>> _PROPERTY_TYPES = new Lazy<IEnumerable<Type>>(() => 
            AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => !type.IsAbstract
                               && type.IsGenericType
                               && typeof(IVariableProperty).IsAssignableFrom(type)
                               && type != typeof(VariableProperty<>))
        );

        private static bool HasSerializableAttribute(this Type type)
        {
            return Attribute.GetCustomAttribute(type, typeof(SerializableAttribute)) != null;
        }
    }
}
