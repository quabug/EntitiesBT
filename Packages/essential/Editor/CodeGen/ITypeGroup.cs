using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditorInternal;

namespace EntitiesBT.Editor
{
    public interface ITypeGroup
    {
        IEnumerable<Type> Types { get; }
    }

    [Serializable]
    public class TypeGroupAssemblies : ITypeGroup
    {
        public AssemblyDefinitionAsset[] Assemblies;

        public IEnumerable<Type> Types
        {
            get
            {
                var assemblyNames = Assemblies.Select(asm => asm.Deserialize().name);
                return Core.Utilities.ALL_ASSEMBLIES.Value
                    .Where(assembly => assemblyNames.Contains(assembly.GetName().Name))
                    .SelectMany(assembly => assembly.GetTypes())
                ;
            }
        }
    }
}
