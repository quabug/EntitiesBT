using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Mono.Cecil;
using Unity.Entities;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;

namespace EntitiesBT.Editor
{
    [InitializeOnLoad]
    public class HackAccessChunkDataUtility
    {
        static HackAccessChunkDataUtility()
        {
            MakeUnityEntitiesVisisbleToEntitiesBT();
        }
        
        // https://stackoverflow.com/a/44329684
        // add [InternalsVisibleTo("EntitiesBT.Runtime")] into assembly of Unity.Entities.dll
        [MenuItem("Tools/EntitiesBT/CecilUnityEntities")]
        public static void MakeUnityEntitiesVisisbleToEntitiesBT()
        {
            // my assembly(EntitiesBT.Runtime) which unable to access internal properties of Unity.Entities.dll
            var btRuntime = Path.GetFileNameWithoutExtension(typeof(EntitiesInternalMethods).Module.Name);
            
            // Unity.Entities.dll
            var entitiesModulePath = typeof(ArchetypeChunk).Module.FullyQualifiedName;
            
            var resolver = new DefaultAssemblyResolver();
            resolver.AddSearchDirectory(Path.GetDirectoryName(entitiesModulePath));
            resolver.AddSearchDirectory(Path.GetDirectoryName(typeof(Application).Module.FullyQualifiedName));
            
            var moduleReaderParameter = new ReaderParameters
            {
                ReadWrite = true
              , AssemblyResolver = resolver
            };
            
            using (var entitiesModule = ModuleDefinition.ReadModule(entitiesModulePath, moduleReaderParameter))
            {
                var visibleToEntitiesBT = new CustomAttribute(entitiesModule.ImportReference(
                    typeof(InternalsVisibleToAttribute).GetConstructor(new[] { typeof(string)})
                ));
                
                var alreadyExist = entitiesModule.Assembly.CustomAttributes
                    .Where(attribute => attribute.AttributeType.FullName == visibleToEntitiesBT.AttributeType.FullName)
                    .Any(attribute => (string)attribute.ConstructorArguments[0].Value == btRuntime)
                ;
                
                if (!alreadyExist)
                {
                    var attributeArgument = new CustomAttributeArgument(entitiesModule.TypeSystem.String, btRuntime);
                    visibleToEntitiesBT.ConstructorArguments.Add(attributeArgument);
                    entitiesModule.Assembly.CustomAttributes.Add(visibleToEntitiesBT);
                    entitiesModule.Write();
                    Debug.Log($"Cecil: Add [InternalsVisibleTo({btRuntime})] into {entitiesModulePath}"); 
                } 
                else
                {
                    Debug.Log($"Cecil: [InternalsVisibleTo({btRuntime})] already exist in {entitiesModulePath}"); 
                }
            }
        }

        // [MenuItem("Tools/EntitiesBT/GenerateDll")]
        // public static void GenerateDll()
        // {
        //     var className = "EntitiesInternalMethods";
        //     
        //     var filePath = EditorUtility.SaveFilePanel("Save Dll", Application.dataPath, className, "dll");
        //     
        //     var dll = AssemblyDefinition.CreateAssembly(
        //         new AssemblyNameDefinition(className, new Version(0, 0, 0, 0)), className, ModuleKind.Dll);
        //
        //     var module = dll.MainModule;
        //
        //     var classAttributes = TypeAttributes.Class
        //                           | TypeAttributes.Public
        //                           | TypeAttributes.Abstract
        //                           | TypeAttributes.Sealed
        //                           | TypeAttributes.AutoClass
        //                           | TypeAttributes.AnsiClass
        //                           | TypeAttributes.BeforeFieldInit
        //     ;
        //
        //     var methodAttributes = MethodAttributes.Public | MethodAttributes.Static | MethodAttributes.HideBySig;
        //     
        //     // create the program type and add it to the module
        //     var classType = new TypeDefinition("EntitiesBT", className, classAttributes, module.TypeSystem.Object);
        //     module.Types.Add(classType);
        //
        //     // // add an empty constructor
        //     // var ctor = new MethodDefinition(".ctor", Mono.Cecil.MethodAttributes.Public | Mono.Cecil.MethodAttributes.HideBySig
        //     //                                                                             | Mono.Cecil.MethodAttributes.SpecialName | Mono.Cecil.MethodAttributes.RTSpecialName, module.TypeSystem.Void);
        //     //
        //     // // create the constructor's method body
        //     // var il = ctor.Body.GetILProcessor();
        //     //
        //     // il.Append(il.Create(OpCodes.Ldarg_0));
        //     //
        //     // // call the base constructor
        //     // il.Append(il.Create(OpCodes.Call, module.Import(typeof(object).GetConstructor(Array.Empty<Type>()))));
        //     //
        //     // il.Append(il.Create(OpCodes.Nop));
        //     // il.Append(il.Create(OpCodes.Ret));
        //     //
        //     // programType.Methods.Add(ctor);
        //
        //     // define the 'Main' method and add it to 'Program'
        //
        //     var il = CreateMethod("GetComponentDataWithTypeRW", typeof(void*), new []
        //     {
        //         ("chunk", typeof(ArchetypeChunk))
        //       , ("chunkIndex", typeof(int))
        //       , ("typeIndex", typeof(int))
        //       , ("globalSystemVersion", typeof(uint))
        //     });
        //     il.Append(il.Create(OpCodes.Nop));
        //
        //     // var writeLineMethod = il.Create(OpCodes.Call,
        //         // module.ImportReference(typeof(Console).GetMethod("WriteLine", new[] { typeof(string) })));
        //
        //     // call the method
        //     // il.Append(writeLineMethod);
        //
        //     il.Append(il.Create(OpCodes.Br_S, ));
        //     il.Append(il.Create(OpCodes.Ldloc_0));
        //     il.Append(il.Create(OpCodes.Ret));
        //
        //     // set the entry point and save the module
        //     dll.Write(filePath);
        //
        //     ILProcessor CreateMethod(string methodName, Type returnType, params (string name, Type type)[] @params)
        //     {
        //         var method = new MethodDefinition(
        //             methodName
        //           , methodAttributes
        //           , module.ImportReference(returnType)
        //         );
        //         classType.Methods.Add(method);
        //         
        //         // create the method body
        //         var methodIL = method.Body.GetILProcessor();
        //         methodIL.Append(methodIL.Create(OpCodes.Nop));
        //
        //         if (@params == null) return methodIL;
        //         for (var i = 0; i < @params.Length; i++)
        //         {
        //             var (name, type) = @params[i];
        //             // add the 'args' parameter
        //             var param = new ParameterDefinition(
        //                 name
        //               , ParameterAttributes.None
        //               , module.ImportReference(type)
        //             );
        //             method.Parameters.Add(param);
        //             methodIL.Append(methodIL.Create(OpCodes.Ldarg, i));
        //         }
        //         return methodIL;
        //     }
        // }
        //
        // public static void ModifyMethods()
        // {
        //     var btRuntime = Path.GetFileNameWithoutExtension(typeof(VirtualMachine).Module.Name);
        //     var entitiesModulePath = typeof(ArchetypeChunk).Module.FullyQualifiedName;
        //     
        //     var resolver = new DefaultAssemblyResolver();
        //     resolver.AddSearchDirectory(Path.GetDirectoryName(entitiesModulePath));
        //     resolver.AddSearchDirectory(Path.GetDirectoryName(typeof(Application).Module.FullyQualifiedName));
        //     
        //     var moduleReaderParameter = new ReaderParameters
        //     {
        //         ReadWrite = true
        //       , AssemblyResolver = resolver
        //     };
        //     
        //     using (var entitiesModule = ModuleDefinition.ReadModule(entitiesModulePath, moduleReaderParameter))
        //     {
        //         var visibleToEntitiesBT = new CustomAttribute(entitiesModule.ImportReference(
        //             typeof(InternalsVisibleToAttribute).GetConstructor(new[] { typeof(string)})
        //         ));
        //         
        //         var alreadyExist = entitiesModule.Assembly.CustomAttributes
        //             .Where(attribute => attribute.AttributeType.FullName == visibleToEntitiesBT.AttributeType.FullName)
        //             .Any(attribute => (string)attribute.ConstructorArguments[0].Value == btRuntime)
        //         ;
        //         
        //         if (!alreadyExist)
        //         {
        //             var attributeArgument = new CustomAttributeArgument(entitiesModule.TypeSystem.String, btRuntime);
        //             visibleToEntitiesBT.ConstructorArguments.Add(attributeArgument);
        //             entitiesModule.Assembly.CustomAttributes.Add(visibleToEntitiesBT);
        //             entitiesModule.Write();
        //             Debug.Log($"Cecil: Add [InternalsVisibleTo({btRuntime})] into {entitiesModulePath}"); 
        //         } 
        //         else
        //         {
        //             Debug.Log($"Cecil: [InternalsVisibleTo({btRuntime})] already exist in {entitiesModulePath}"); 
        //         }
        //     }
        // }
        //
    }
}
