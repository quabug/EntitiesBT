using System.Collections.Generic;
using System.IO;
using System.Linq;
using EntitiesBT.Core;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using Unity.CompilationPipeline.Common.Diagnostics;
using Unity.CompilationPipeline.Common.ILPostProcessing;

namespace EntitiesBT.CodeGen.Editor
{
     public class NodeAccessorAttributesILPostProcessor : ILPostProcessor
     {
          public override ILPostProcessor GetInstance()
          {
               return this;
          }

          public override bool WillProcess(ICompiledAssembly compiledAssembly)
          {
              var editorAssemblyName = typeof(NodeAccessorAttributesILPostProcessor).Assembly.GetName().Name;
              var runtimeAssemblyName = typeof(INodeData).Assembly.GetName().Name;
              if (compiledAssembly.Name == editorAssemblyName) return false;
              if (compiledAssembly.Name == runtimeAssemblyName) return true;
              return compiledAssembly.References.Any(f => Path.GetFileNameWithoutExtension(f) == runtimeAssemblyName);
          }

          public override ILPostProcessResult Process(ICompiledAssembly compiledAssembly)
          {
              var logger = new ILPostProcessorLogger(new List<DiagnosticMessage>());
              logger.Debug($"process {compiledAssembly.Name}({string.Join("|", compiledAssembly.References.Select(Path.GetFileName).Where(f => !new [] {"System", "Mono", "mscorlib", "netstandard", "Microsoft", "Unity", "UnityEngine"}.Any(f.StartsWith)))})");

              using var resolver = new PostProcessorAssemblyResolver(compiledAssembly.References);
              using var assemblyDefinition = compiledAssembly.LoadAssembly(resolver);

              var modified = false;

              var nodes = assemblyDefinition.MainModule.GetAllTypes()
                  .Where(type => type.IsClass && !type.IsAbstract && type.TypeImplements(typeof(INodeData)))
                  .ToArray()
              ;

              var methods = nodes.SelectMany(FetchNodeDataMethods)
                  .Where(method => method != null && !method.CustomAttributes.FindAccessorAttributes().Any())
              ;

              foreach (var method in methods)
              {
                  var attributes = method.GenerateAccessorAttributes();
                  if (!attributes.Any()) continue;

                  modified = true;
                  method.CustomAttributes.AddRange(attributes);
              }

              return modified ? assemblyDefinition.Write(logger.Messages) : new ILPostProcessResult(null, logger.Messages);
          }

          private IEnumerable<MethodDefinition> FetchNodeDataMethods(TypeDefinition type)
          {
              yield return type.GetMethod(nameof(INodeData.Tick));
              yield return type.GetMethodNullable(nameof(ICustomResetAction.Reset));
          }
     }
}
