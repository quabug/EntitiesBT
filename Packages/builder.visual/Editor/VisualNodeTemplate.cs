using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EntitiesBT.Core;
using EntitiesBT.Editor;

namespace EntitiesBT.Builder.Visual.Editor
{
    internal class VisualNodeTemplate : INodeCodeTemplate
    {
        public string Header => "// automatically generate from `VisualNodeTemplateCode.cs`";
        public string Generate(Type nodeType, IEnumerable<INodeDataFieldCodeGenerator> fieldGenerators, string classNameOverride = "")
        {
            var behaviorNodeType = nodeType.GetCustomAttribute<BehaviorNodeAttribute>().Type;
            var hasChildren = behaviorNodeType != BehaviorNodeType.Action;
            var fields = nodeType.GetFields(BindingFlags.Public | BindingFlags.Instance);
            var fieldStrings = fields.Select(fi => fieldGenerators.First(gen => gen.ShouldGenerate(fi)).GenerateField(fi));
            var buildStrings = fields.Select(fi => fieldGenerators.First(gen => gen.ShouldGenerate(fi)).GenerateBuild(fi));
            var className = string.IsNullOrEmpty(classNameOverride) ? nodeType.Name : classNameOverride;
            return $@"{Header}
using EntitiesBT.Core;
using EntitiesBT.Nodes;
using EntitiesBT.Components;
using Unity.Entities;
using System;
using Runtime;

namespace EntitiesBT.Builder.Visual
{{
    [NodeSearcherItem(""EntitiesBT/Node/{nodeType.Name}"")]
    [Serializable]
    public class {className} : IVisualBuilderNode
    {{
        [PortDescription("""")] public InputTriggerPort Parent;
        {(hasChildren ? "[PortDescription(\"\")] public OutputTriggerPort Children;" : "")}

        {string.Join(Environment.NewLine + "        ", fieldStrings)}

        public INodeDataBuilder GetBuilder(GraphInstance instance, GraphDefinition definition)
        {{
            var @this = this;
            return new VisualBuilder<{nodeType.FullName}>(BuildImpl, {(hasChildren ? "Children.ToBuilderNode(instance, definition)" : "null")});
            void BuildImpl(BlobBuilder blobBuilder, ref {nodeType.FullName} data, INodeDataBuilder self, ITreeNode<INodeDataBuilder>[] builders)
            {{
                {string.Join(Environment.NewLine + "                ", buildStrings)}
            }}
        }}
    }}
}}
";
        }
    }
}
