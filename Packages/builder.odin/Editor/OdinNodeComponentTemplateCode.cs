using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EntitiesBT.Editor
{
    internal class OdinNodeComponentTemplate : INodeCodeTemplate
    {
        public string Header => "// automatically generate from `OdinNodeComponentTemplate.cs`";
        public string Generate(Type nodeType, IEnumerable<INodeDataFieldCodeGenerator> fieldGenerators, string classNameOverride = "")
        {
            var fields = nodeType.GetFields(BindingFlags.Public | BindingFlags.Instance);
            var fieldStrings = fields.Select(fi => fieldGenerators.First(gen => gen.ShouldGenerate(fi)).GenerateField(fi));
            var buildStrings = fields.Select(fi => fieldGenerators.First(gen => gen.ShouldGenerate(fi)).GenerateBuild(fi));
            var className = string.IsNullOrEmpty(classNameOverride) ? nodeType.Name : classNameOverride;
            return $@"{Header}
#if ODIN_INSPECTOR

using EntitiesBT.Core;
using Unity.Entities;
using Sirenix.Serialization;
using System;
using EntitiesBT.Variant;
using Sirenix.OdinInspector;

namespace EntitiesBT.Components.Odin
{{
    public class {className} : OdinNode<{nodeType.FullName}>
    {{
        {string.Join(Environment.NewLine + "        ", fieldStrings)}
        protected override void Build(ref {nodeType.FullName} data, BlobBuilder builder, ITreeNode<INodeDataBuilder>[] tree)
        {{
            {string.Join(Environment.NewLine + "            ", buildStrings)}
        }}
    }}
}}

#endif
";
        }
    }
}
