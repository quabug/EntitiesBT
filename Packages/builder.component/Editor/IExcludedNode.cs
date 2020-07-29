using System;
using System.Linq;
using System.Reflection;
using EntitiesBT.Variable;

namespace EntitiesBT.Editor
{
    public interface IExcludedNode
    {
        bool IsExcluded(Type node);
    }

    [Serializable]
    public class ExcludedNodeWithCustomName : IExcludedNode
    {
        public string[] NodeNames = {};

        public bool IsExcluded(Type node)
        {
            return NodeNames.Contains(node.FullName);
        }
    }

    public class ExcludedNodeWithoutBlobVariable : IExcludedNode
    {
        public bool IsExcluded(Type node)
        {
            var fields = node.GetFields(BindingFlags.Public | BindingFlags.Instance);
            return !fields.Select(fi => fi.FieldType)
                .Any(type => type.IsGenericType && type.GetGenericTypeDefinition() == typeof(BlobVariable<>))
            ;
        }
    }
}
