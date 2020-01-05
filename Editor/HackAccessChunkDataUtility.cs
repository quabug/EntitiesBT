using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using EntitiesBT.Core;
using Mono.Cecil;
using Unity.Entities;
using UnityEditor;
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
        // [DidReloadScripts]
        [MenuItem("Tools/EntitiesBT/CecilUnityEntities")]
        public static void MakeUnityEntitiesVisisbleToEntitiesBT()
        {
            // my assembly(EntitiesBT.Runtime) which unable to access internal properties of Unity.Entities.dll
            var btRuntime = Path.GetFileNameWithoutExtension(typeof(VirtualMachine).Module.Name);
            
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
            
            Debug.Log($"Cecil: Add [InternalsVisibleTo({btRuntime})] into {entitiesModulePath}"); 
            
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
                }
            }
        }
    }
}
