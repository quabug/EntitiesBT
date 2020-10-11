using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using EntitiesBT.Core;
using Unity.Assertions;
using UnityEditor;
using UnityEngine;

namespace EntitiesBT.Editor
{
    [CreateAssetMenu(fileName = "NodeCodeGenerator", menuName = "EntitiesBT/NodeCodeGenerator")]
    public class NodeComponentsGenerator : ScriptableObject
    {
        [SerializeReference, SerializeReferenceButton] private INodeCodeTemplate _codeTemplate;
        [SerializeField] private string _outputDirectory = default;
        [SerializeField] private string _classRenameRegex = @"(\w+)(?:Node)/$1";
        [SerializeField] private string[] _includedNodeAssemblies = default;
        [SerializeReference, SerializeReferenceButton] private IExcludedNode[] _excludedNodes =
            { new ExcludedNodeWithCustomName() };
        [SerializeReference, SerializeReferenceButton] private INodeDataFieldCodeGenerator[] _fieldCodeGenerators =
            { new DefaultNodeDataFieldCodeGenerator() };

        [ContextMenu("Generate")]
        public void GenerateComponents()
        {
            Assert.IsNotNull(_codeTemplate);

            var currentDirectory = Path.GetDirectoryName(AssetDatabase.GetAssetPath(this));
            var scriptDirectory = $"{currentDirectory}/{_outputDirectory}";
            var regexList = _classRenameRegex.Split('/');
            var classNameRegex = new Regex(regexList.Length > 0 ? regexList[0] : @"(*)");
            var replaceName = regexList.Length > 1 ? regexList[1] : "$1";

            foreach (var nodeType in AppDomain.CurrentDomain.GetAssemblies()
                .Where(assembly => _includedNodeAssemblies.Contains(assembly.GetName().Name))
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => typeof(INodeData).IsAssignableFrom(type) && type.IsValueType)
                .Where(type => !_excludedNodes.Any(check => check.IsExcluded(type)))
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
