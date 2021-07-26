using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using Unity.CompilationPipeline.Common.Diagnostics;
using Unity.CompilationPipeline.Common.ILPostProcessing;

namespace EntitiesBT.CodeGen.Editor
{
    internal static class EnumerableExtension
    {
        public static IEnumerable<T> Yield<T>(this T value)
        {
            yield return value;
        }

        public static int FindLastIndexOf<T>(this IList<T> list, Predicate<T> predicate)
        {
            for (var i = list.Count - 1; i >= 0; i--)
            {
                if (predicate(list[i]))
                    return i;
            }
            return -1;
        }
    }

    internal static class PostProcessorExtension
    {
        public static AssemblyDefinition LoadAssembly(this ICompiledAssembly compiledAssembly, IAssemblyResolver resolver)
        {
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

        public static IEnumerable<AssemblyDefinition> LoadLibraryAssemblies(this ICompiledAssembly compiledAssembly, PostProcessorAssemblyResolver resolver)
        {
            return compiledAssembly.References.Where(name => name.StartsWith("Library")).Select(resolver.Resolve);
        }

        public static ILPostProcessorLogger CreateLogger(this AssemblyDefinition assembly)
        {
            var logger = new ILPostProcessorLogger(new List<DiagnosticMessage>());
            // var loggerAttributes = assembly.GetAttributesOf<GenericSerializeReferenceLoggerAttribute>();
            // if (loggerAttributes.Any()) logger.LogLevel = (LogLevel)loggerAttributes.First().ConstructorArguments[0].Value;
            return logger;
        }
    }
}