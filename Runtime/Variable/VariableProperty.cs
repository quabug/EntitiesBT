using System.Reflection;
using Unity.Entities;

namespace EntitiesBT.Variable
{
    public abstract class VariableProperty
    {
        public const BindingFlags FIELD_BINDING_FLAGS = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        public abstract int VariablePropertyTypeId { get; }
    }

    public abstract class VariableProperty<T> : VariableProperty where T : struct
    {
        public virtual void Allocate(ref BlobBuilder builder, ref BlobVariable<T> blobVariable)
        {
            blobVariable.VariableId = VariablePropertyTypeId;
            AllocateData(ref builder, ref blobVariable);
        }
        protected abstract void AllocateData(ref BlobBuilder builder, ref BlobVariable<T> blobVariable);
    }
}
