using System.Reflection;
using Unity.Entities;

namespace EntitiesBT.Variable
{
    public interface IVariableProperty
    {
        int VariablePropertyTypeId { get; }
    }

    public class VariableProperty<T> : IVariableProperty where T : struct
    {
        public const BindingFlags FIELD_BINDING_FLAGS = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        
        public virtual void Allocate(ref BlobBuilder builder, ref BlobVariable<T> blobVariable)
        {
            blobVariable.VariableId = VariablePropertyTypeId;
            AllocateData(ref builder, ref blobVariable);
        }
        protected virtual void AllocateData(ref BlobBuilder builder, ref BlobVariable<T> blobVariable) {}
        public virtual int VariablePropertyTypeId => 0;
    }
}
