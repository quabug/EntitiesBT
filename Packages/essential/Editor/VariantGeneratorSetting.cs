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
        }

        [ContextMenu("CreateScript-InterfaceOnly")]
        public void CreateInterfaceScript()
        {
            RecursiveFindReferences();
            var filePath = SaveFilePanel("Save Script", Directory, $"{Filename}-Interface", "cs");
        }

        [ContextMenu("CreateScript-ClassesOnly")]
        public void CreateClassesScript()
        {
            RecursiveFindReferences();
            var filePath = SaveFilePanel("Save Script", Directory, $"{Filename}-Classes", "cs");
        }

        private string Directory => Path.GetDirectoryName(AssetDatabase.GetAssetPath(this));

        private void RecursiveFindReferences()
        {
            _referenceAssemblies = Assembly.ToAssembly().FindReferenceAssemblies();
        }

        private bool IsReferenceType(Type type) => _referenceAssemblies.Contains(type.Assembly);
    }
}
