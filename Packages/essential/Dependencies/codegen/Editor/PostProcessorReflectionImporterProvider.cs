using System.Linq;
using System.Reflection;
using Mono.Cecil;

namespace EntitiesBT.CodeGen.Editor
{
    internal class PostProcessorReflectionImporterProvider : IReflectionImporterProvider
    {
        public IReflectionImporter GetReflectionImporter(ModuleDefinition module)
        {
            return new PostProcessorReflectionImporter(module);
        }

        class PostProcessorReflectionImporter : DefaultReflectionImporter
        {
            private const string SystemPrivateCoreLib = "System.Private.CoreLib";
            private AssemblyNameReference _correctCorlib;

            public PostProcessorReflectionImporter(ModuleDefinition module) : base(module)
            {
                _correctCorlib = module.AssemblyReferences.FirstOrDefault(a => a.Name == "mscorlib" || a.Name == "netstandard" || a.Name == SystemPrivateCoreLib);
            }

            public override AssemblyNameReference ImportReference(AssemblyName reference)
            {
                if (_correctCorlib != null && reference.Name == SystemPrivateCoreLib)
                    return _correctCorlib;

                return base.ImportReference(reference);
            }
        }
    }
}