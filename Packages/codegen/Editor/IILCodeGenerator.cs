using Mono.Cecil;

namespace EntitiesBT.CodeGen.Editor
{
    public interface IILCodeGenerator
    {
        bool Generate(AssemblyDefinition assemblyDefinition);
        int Order { get; }
        ILogger Logger { set; }
    }
}