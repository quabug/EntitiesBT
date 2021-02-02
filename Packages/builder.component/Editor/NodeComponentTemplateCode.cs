using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EntitiesBT.Editor
{
    internal class NodeComponentTemplate : INodeCodeTemplate
    {
        public string Header => "// automatically generate from `NodeComponentTemplateCode.cs`";
        public string Generate(Type nodeType, IEnumerable<INodeDataFieldCodeGenerator> fieldGenerators, string classNameOverride = "")
        {
            var fields = nodeType.GetFields(BindingFlags.Public | BindingFlags.Instance);
            var fieldStrings = fields.Select(fi => fieldGenerators.First(gen => gen.ShouldGenerate(fi)).GenerateField(fi));
            var buildStrings = fields.Select(fi => fieldGenerators.First(gen => gen.ShouldGenerate(fi)).GenerateBuild(fi));
            var className = string.IsNullOrEmpty(classNameOverride) ? nodeType.Name : classNameOverride;
            return $@"{Header}
using EntitiesBT.Core;
using EntitiesBT.Attributes;
using Unity.Entities;
using static EntitiesBT.Variant.Utilities;

namespace EntitiesBT.Components
{{
    public class {className} : BTNode<{nodeType.FullName}>
    {{
        {string.Join(Environment.NewLine + "        ", fieldStrings)}
        protected override unsafe void Build(ref {nodeType.FullName} data, BlobBuilder builder, ITreeNode<INodeDataBuilder>[] tree)
        {{
            {string.Join(Environment.NewLine + "            ", buildStrings)}
        }}
    }}
}}
";
        }
    }
}
