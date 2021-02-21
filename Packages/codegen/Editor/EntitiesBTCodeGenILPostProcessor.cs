using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EntitiesBT.Core;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Unity.CompilationPipeline.Common.Diagnostics;
using Unity.CompilationPipeline.Common.ILPostProcessing;

namespace EntitiesBT.CodeGen.Editor
{
     public class EntitiesBTCodeGenILPostProcessor : ILPostProcessor
     {
          private static readonly Lazy<IReadOnlyList<IILCodeGenerator>> _CODE_GENERATOR =
               new Lazy<IReadOnlyList<IILCodeGenerator>>(
                    () => AppDomain.CurrentDomain.CreateInstances<IILCodeGenerator>()
                         .OrderBy(generator => generator.Order)
                         .ToArray()
               );

          private static readonly Lazy<string> _EDITOR_ASSEMBLY_NAME = new Lazy<string>(() =>
               typeof(EntitiesBTCodeGenILPostProcessor).Assembly.GetName().Name);

          private static readonly Lazy<string> _RUNTIME_ASSEMBLY_NAME = new Lazy<string>(() =>
               typeof(INodeData).Assembly.GetName().Name);

          public override ILPostProcessor GetInstance()
          {
               return this;
          }

          public override bool WillProcess(ICompiledAssembly compiledAssembly)
          {
               if (compiledAssembly.Name == _EDITOR_ASSEMBLY_NAME.Value) return false;
               if (compiledAssembly.Name == _RUNTIME_ASSEMBLY_NAME.Value) return true;
               return compiledAssembly.References.Any(f => Path.GetFileNameWithoutExtension(f) == _RUNTIME_ASSEMBLY_NAME.Value);
          }

          public override ILPostProcessResult Process(ICompiledAssembly compiledAssembly)
          {
               var logger = new ILPostProcessorLogger(new List<DiagnosticMessage>());
               logger.Debug($"process {compiledAssembly.Name}({string.Join("|", compiledAssembly.References.Select(Path.GetFileName).Where(f => !new [] {"System", "Mono", "mscorlib", "netstandard", "Microsoft", "Unity", "UnityEngine"}.Any(f.StartsWith)))}) [{_CODE_GENERATOR.Value.Count}]");
               using var assemblyDefinition = LoadAssemblyDefinition(compiledAssembly);

               var modified = false;
               foreach (var generator in _CODE_GENERATOR.Value)
               {
                    try
                    {
                         generator.Logger = logger;
                         var m = generator.Generate(assemblyDefinition);
                         modified = modified || m;
                    }
                    catch (Exception ex)
                    {
                         logger.Error(ex.Message);
                         break;
                    }
               }

               if (!modified) return new ILPostProcessResult(null, logger.Messages);

               var pe = new MemoryStream();
               var pdb = new MemoryStream();
               var writerParameters = new WriterParameters
               {
                   SymbolWriterProvider = new PortablePdbWriterProvider()
                   , SymbolStream = pdb
                   , WriteSymbols = true
               };
               assemblyDefinition.Write(pe, writerParameters);
               return new ILPostProcessResult(new InMemoryAssembly(pe.ToArray(), pdb.ToArray()), logger.Messages);
          }

          private static AssemblyDefinition LoadAssemblyDefinition(ICompiledAssembly compiledAssembly)
          {
               var resolver = new PostProcessorAssemblyResolver(compiledAssembly.References);
               var symbolStream = new MemoryStream(compiledAssembly.InMemoryAssembly.PdbData.ToArray());
               var readerParameters = new ReaderParameters
               {
                    SymbolStream = symbolStream,
                    SymbolReaderProvider = new PortablePdbReaderProvider(),
                    AssemblyResolver = resolver,
                    ReflectionImporterProvider = new PostProcessorReflectionImporterProvider(),
                    ReadingMode = ReadingMode.Immediate,
               };
               var peStream = new MemoryStream(compiledAssembly.InMemoryAssembly.PeData.ToArray());
               return AssemblyDefinition.ReadAssembly(peStream, readerParameters);
          }
     }
}
