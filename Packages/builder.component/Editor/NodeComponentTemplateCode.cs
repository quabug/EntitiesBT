using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EntitiesBT.Editor
{
    internal static class NodeComponentTemplate
    {
        internal const string HEAD_LINE = "// automatically generate from `NodeComponentTemplateCode.cs`";

        internal static string GenerateComponentScript(
            this Type nodeType
          , IEnumerable<INodeDataFieldCodeGenerator> fieldGenerators
          , string classNameOverride = ""
        )
        {
            var fields = nodeType.GetFields(BindingFlags.Public | BindingFlags.Instance);
            var fieldStrings = fields.Select(fi => fieldGenerators.First(gen => gen.ShouldGenerate(fi)).GenerateField(fi));
            var buildStrings = fields.Select(fi => fieldGenerators.First(gen => gen.ShouldGenerate(fi)).GenerateBuild(fi));
            var className = string.IsNullOrEmpty(classNameOverride) ? nodeType.Name : classNameOverride;
            return $@"{HEAD_LINE}
using EntitiesBT.Core;
using Unity.Entities;

namespace EntitiesBT.Components
{{
    public class {className} : BTNode<{nodeType.FullName}>
    {{
        {string.Join(Environment.NewLine + "        ", fieldStrings)}
        protected override void Build(ref {nodeType.FullName} data, BlobBuilder builder, ITreeNode<INodeDataBuilder>[] tree)
        {{
            {string.Join(Environment.NewLine + "            ", buildStrings)}
        }}
    }}
}}
";
        }

    }
}
