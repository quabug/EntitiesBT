using JetBrains.Annotations;
using Unity.Build;
using UnityEditorInternal;

namespace EntitiesBT.Editor
{
    public class AssemblyDefinitionDescription
    {
        public string name = null;
        public string[] references = null;
        public string[] includePlatforms = null;
        public string[] excludePlatforms = null;
        public string[] optionalUnityReferences = null;
    }

    public static class AssemblyExtension
    {
        public static AssemblyDefinitionDescription Deserialize([NotNull] this AssemblyDefinitionAsset asset)
        {
            var assetPath = UnityEditor.AssetDatabase.GetAssetPath(asset);
            var assemblyDefinition = BuildAssemblyCache.AssemblyDefinitionDescriptionUtility.Deserialize(assetPath);
            return new AssemblyDefinitionDescription
            {
                name = assemblyDefinition.name
              , references = assemblyDefinition.references
              , includePlatforms = assemblyDefinition.includePlatforms
              , excludePlatforms = assemblyDefinition.excludePlatforms
              , optionalUnityReferences = assemblyDefinition.optionalUnityReferences
            };
        }
    }
}
