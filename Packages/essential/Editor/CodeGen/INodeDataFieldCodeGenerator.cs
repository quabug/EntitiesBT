using System.Reflection;

namespace EntitiesBT.Editor
{
    public interface INodeDataFieldCodeGenerator
    {
        bool ShouldGenerate(FieldInfo fi);
        string GenerateField(FieldInfo fi);
        string GenerateBuild(FieldInfo fi);
    }

    public class DefaultNodeDataFieldCodeGenerator : INodeDataFieldCodeGenerator
    {
        public bool ShouldGenerate(FieldInfo fi)
        {
            return true;
        }

        public string GenerateField(FieldInfo fi)
        {
            return $"public {fi.FieldType.FullName.ToCodeName()} {fi.Name};";
        }

        public string GenerateBuild(FieldInfo fi)
        {
            return $"data.{fi.Name} = {fi.Name};";
        }
    }

}
