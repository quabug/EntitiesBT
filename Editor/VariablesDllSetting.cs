using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EntitiesBT.Variable;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using Unity.Entities;
using UnityEditor;
using UnityEngine;
using MethodAttributes = Mono.Cecil.MethodAttributes;
using ParameterAttributes = Mono.Cecil.ParameterAttributes;
using TypeAttributes = Mono.Cecil.TypeAttributes;

namespace EntitiesBT.Editor
{
    [CreateAssetMenu(fileName = "VariablesDllSetting", menuName = "EntitiesBT/VariableDllSetting")]
    public class VariablesDllSetting : ScriptableObject
    {
        public string[] Types;
        public string Filename = "VariableProperties";
        public string Namespace = "EntitiesBT.Variable";

        [ContextMenu("CreateDll")]
        public void Create()
        {
            var filePath = EditorUtility.SaveFilePanel("Save Dll", Application.dataPath, $"{Namespace}.{Filename}", "dll");
            VariableDllGenerator.CreateDll(Filename, filePath, Namespace, Types);
        }
    }
    
    public static class VariableDllGenerator
    {
        public static void CreateDll(string filename, string filepath, string namespaceName, string[] types)
        {
            var dll = AssemblyDefinition.CreateAssembly(
                new AssemblyNameDefinition(filename, new Version(0, 0, 0, 0)), filename, ModuleKind.Dll
            );
            var module = dll.MainModule;
            foreach (var type in _VALUE_TYPES.Value)
            {
                if (types.Contains(type.FullName))
                    module.CreatePropertyInterface(type, namespaceName);
            }
            dll.Write(filepath);
        }
        
        private static void CreatePropertyInterface(this ModuleDefinition module, Type type, string namespaceName)
        {
// public interface Int32Property
// {
//     void Allocate(ref BlobBuilder builder, ref BlobVariable<Int32> blobVariable);
// }
// .class interface public abstract auto ansi
//   EntitiesBT.Editor.Int32Property
// {
//
//   .method public hidebysig virtual newslot abstract instance void
//     Allocate(
//       valuetype [Unity.Entities]Unity.Entities.BlobBuilder& builder,
//       valuetype [EntitiesBT.Runtime]EntitiesBT.Variable.BlobVariable`1<int32>& blobVariable
//     ) cil managed
//   {
//     // Can't find a body
//   } // end of method Int32Property::Allocate
// } // end of class EntitiesBT.Editor.Int32Property
            
            const TypeAttributes interfaceAttributes = TypeAttributes.Class
                                                       | TypeAttributes.Interface
                                                       | TypeAttributes.Public
                                                       | TypeAttributes.Abstract
                                                       | TypeAttributes.AutoClass
                                                       | TypeAttributes.AnsiClass;
            var interfaceType = new TypeDefinition(namespaceName, $"{type.Name}Property", interfaceAttributes, module.TypeSystem.Object);
            
            const MethodAttributes methodAttributes = MethodAttributes.Public
                                                      | MethodAttributes.HideBySig
                                                      | MethodAttributes.Virtual
                                                      | MethodAttributes.NewSlot
                                                      | MethodAttributes.Abstract;
            var allocateMethod = new MethodDefinition("Allocate", methodAttributes, module.TypeSystem.Void);
            var builderParameter = new ParameterDefinition(
                "builder"
              , ParameterAttributes.None
              , module.ImportReference(typeof(BlobBuilder)).MakeByReferenceType()
            );
            var blobVariableParameter = new ParameterDefinition(
                "blobVariable"
              , ParameterAttributes.None
              , module.ImportReference(typeof(BlobVariable<>).MakeGenericType(type)).MakeByReferenceType()
            );
            
            allocateMethod.Parameters.Add(builderParameter);
            allocateMethod.Parameters.Add(blobVariableParameter);
            interfaceType.Methods.Add(allocateMethod);
            module.Types.Add(interfaceType);

            foreach (var propertyType in _PROPERTY_TYPES.Value)
            {
                module.CreatePropertyClass(namespaceName, type, propertyType, new InterfaceImplementation(interfaceType));
            }
        }

        private static void CreatePropertyClass(this ModuleDefinition module, string namespaceName, Type valueType, Type propertyType, InterfaceImplementation @interface)
        {
// .class public auto ansi beforefieldinit
//   EntitiesBT.Editor.Int32CustomViariableProperty
//     extends class [EntitiesBT.Runtime]EntitiesBT.Variable.CustomVariableProperty`1<int32>
//     implements EntitiesBT.Editor.Int32Property
// {
// 
//   .method public hidebysig specialname rtspecialname instance void
//     .ctor() cil managed
//   {
//     .maxstack 8
// 
//     IL_0000: ldarg.0      // this
//     IL_0001: call         instance void class [EntitiesBT.Runtime]EntitiesBT.Variable.CustomVariableProperty`1<int32>::.ctor()
//     IL_0006: nop
//     IL_0007: ret
// 
//   } // end of method Int32CustomViariableProperty::.ctor
// } // end of class EntitiesBT.Editor.Int32CustomViariableProperty

            const TypeAttributes classAttributes = TypeAttributes.Class | TypeAttributes.Public | TypeAttributes.AutoClass | TypeAttributes.AnsiClass | TypeAttributes.BeforeFieldInit;
            var baseType = propertyType.MakeGenericType(valueType);
            var classType = new TypeDefinition(namespaceName, $"{valueType.Name}{propertyType.Name}", classAttributes, module.ImportReference(baseType));
            classType.Interfaces.Add(@interface);

            const MethodAttributes methodAttributes = MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName;
            var ctor = new MethodDefinition(".ctor", methodAttributes, module.TypeSystem.Void);
            var il = ctor.Body.GetILProcessor();
            il.Append(il.Create(OpCodes.Ldarg_0));
            // call the base constructor
            il.Append(il.Create(OpCodes.Call, module.ImportReference(baseType.GetConstructor(Array.Empty<Type>()))));
            il.Append(il.Create(OpCodes.Nop));
            il.Append(il.Create(OpCodes.Ret));
            
            var serializableAttribute = new CustomAttribute(module.ImportReference(
                typeof(SerializableAttribute).GetConstructor(Array.Empty<Type>())
            ));
            classType.CustomAttributes.Add(serializableAttribute);
            classType.Methods.Add(ctor);
            module.Types.Add(classType);
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
                .Where(type => !type.IsAbstract && type.IsGenericType && type.IsSubclassOf(typeof(VariableProperty)))
        );

        private static bool HasSerializableAttribute(this Type type)
        {
            return Attribute.GetCustomAttribute(type, typeof(SerializableAttribute)) != null;
        }

        private static bool IsEditorAssembly(this Assembly assembly)
        {
            return Attribute.GetCustomAttribute(assembly, typeof(AssemblyIsEditorAssembly)) != null;
        }
    }
}
