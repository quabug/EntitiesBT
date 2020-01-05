using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using EntitiesBT.Core;
using Mono.Cecil;
using Unity.Entities;
using UnityEditor.Callbacks;
using UnityEngine;

namespace EntitiesBT.Editor
{
    public class HackAccessChunkDataUtility
    {
        // https://stackoverflow.com/a/44329684
        [DidReloadScripts]
        public static void OnScriptReload()
        {
            var btRuntime = Path.GetFileNameWithoutExtension(typeof(VirtualMachine).Module.Name);
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
                var visibleToEntitiesBT = new CustomAttribute(entitiesModule.Import(
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
