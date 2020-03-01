using System.Reflection;
using Unity.Entities;

namespace EntitiesBT.Variable
{
    public abstract class VariableProperty
    {
        public const BindingFlags FIELD_BINDING_FLAGS = BindingFlags.Public | BindingFlags.NonPublic;
    }

    public class VariableProperty<T> : VariableProperty where T : struct
    {
        public virtual void Allocate(ref BlobBuilder builder, ref BlobVariable<T> blobVariable) {}
    }
}
