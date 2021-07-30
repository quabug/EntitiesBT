using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using EntitiesBT.Core;
using Mono.Cecil;
using Mono.Cecil.Rocks;

namespace EntitiesBT.CodeGen.Editor
{
    public class NodeAccessorAttributeILPostProcessor : IILCodeGenerator
    {
        public int Order { get; }
        public ILogger Logger { get; set; }

        public bool Generate(AssemblyDefinition assemblyDefinition)
        {
            var sw = Stopwatch.StartNew();
            try
            {
                var nodes = assemblyDefinition.MainModule.GetAllTypes()
                    .Where(type => type.IsClass && !type.IsAbstract && type.TypeImplements(typeof(INodeData)))
                    .ToArray()
                ;

                var accessorGenerated = GenerateAccessorAttributes(nodes);
                return accessorGenerated;
            }
            finally
            {
                Logger.Debug($"generate accessor attributes for {assemblyDefinition.Name} ({sw.ElapsedMilliseconds}ms)");
            }

            bool GenerateAccessorAttributes(IList<TypeDefinition> nodes)
            {
                var methods = nodes.SelectMany(FetchNodeDataMethods)
                    .Where(method => method != null && !method.CustomAttributes.FindAccessorAttributes().Any())
                ;
                var modified = false;
                foreach (var method in methods)
                {
                    var attributes = method.GenerateAccessorAttributes();
                    if (!attributes.Any()) continue;

                    Logger.Debug($"generate accessor attributes for {method.FullName}");
                    modified = true;
                    method.CustomAttributes.AddRange(attributes);
                }
                return modified;

                IEnumerable<MethodDefinition> FetchNodeDataMethods(TypeDefinition type)
                {
                    yield return type.GetMethod(nameof(INodeData.Tick));
                    yield return type.GetMethodNullable(nameof(ICustomResetAction.Reset));
                }
            }
        }
    }
}