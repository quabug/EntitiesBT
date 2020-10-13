using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using EntitiesBT.Core;
using Unity.Assertions;
using UnityEditor;
using UnityEngine;

#if ODIN_INSPECTOR
using DrawWithUnityAttribute = Sirenix.OdinInspector.DrawWithUnityAttribute;
#else
using DrawWithUnityAttribute = EntitiesBT.Core.DrawWithUnityAttribute;
#endif

namespace EntitiesBT.Editor
{
    [CreateAssetMenu(fileName = "NodeCodeGenerator", menuName = "EntitiesBT/NodeCodeGenerator")]
    public class NodeCodeGenerator : ScriptableObject
    {
        [SerializeReference, SerializeReferenceButton, DrawWithUnity]
        private INodeCodeTemplate _codeTemplate;

        [SerializeField]
        private string _outputDirectory = "Node";

        [SerializeField]
        private string _classRenameRegex = @"(\w+)(?:Node)/$1";

        [SerializeReference, SerializeReferenceButton, DrawWithUnity]
        private ITypeGroup[] _includedNodes = { new TypeGroupAssemblies() };

        [SerializeReference, SerializeReferenceButton, DrawWithUnity]
        private ITypeValidator[] _excludedNodes = { new TypeValidatorWithFullName() };

        [SerializeReference, SerializeReferenceButton, DrawWithUnity]
        private INodeDataFieldCodeGenerator[] _fieldCodeGenerators = { new DefaultNodeDataFieldCodeGenerator() };

        [ContextMenu("Generate")]
        public void GenerateComponents()
        {
            Assert.IsNotNull(_codeTemplate);

            var currentDirectory = Path.GetDirectoryName(AssetDatabase.GetAssetPath(this));
            var scriptDirectory = $"{currentDirectory}/{_outputDirectory}";
            var regexList = _classRenameRegex.Split('/');
            var classNameRegex = new Regex(regexList.Length > 0 ? regexList[0] : @"(*)");
            var replaceName = regexList.Length > 1 ? regexList[1] : "$1";

            foreach (var nodeType in _includedNodes.SelectMany(include => include.Types)
                .Where(type => typeof(INodeData).IsAssignableFrom(type) && type.IsValueType)
                .Where(type => !_excludedNodes.Any(check => check.Verify(type)))
            )
            {
                var className = classNameRegex.Replace(nodeType.Name, replaceName);
                var filepath = $"{scriptDirectory}/{className}.cs";
                if (!Directory.Exists(scriptDirectory)) Directory.CreateDirectory(scriptDirectory);
                if (!File.Exists(filepath) || File.ReadLines(filepath).FirstOrDefault() == _codeTemplate.Header)
                {
                    var script = _codeTemplate.Generate(nodeType, _fieldCodeGenerators, className);
                    using (var writer = new StreamWriter(filepath)) writer.Write(script);
                }
            }
            AssetDatabase.Refresh();
        }
    }
}
