using System.Reflection;
using EntitiesBT.Core;
using Unity.Entities;

namespace EntitiesBT.Variable
{
    public interface IVariableProperty
    {
        int VariablePropertyTypeId { get; }
    }

    public abstract class VariableProperty<T> : IVariableProperty where T : struct
    {
        public const BindingFlags FIELD_BINDING_FLAGS = BindingFlags.Instance | BindingFlags.Public;
        
        public virtual void Allocate(ref BlobBuilder builder, ref BlobVariable<T> blobVariable, INodeDataBuilder self, ITreeNode<INodeDataBuilder>[] tree)
        {
            blobVariable.VariableId = VariablePropertyTypeId;
            AllocateData(ref builder, ref blobVariable, self, tree);
        }
        protected virtual void AllocateData(ref BlobBuilder builder, ref BlobVariable<T> blobVariable, INodeDataBuilder self, ITreeNode<INodeDataBuilder>[] tree) {}
        public virtual int VariablePropertyTypeId => 0;
    }
}
