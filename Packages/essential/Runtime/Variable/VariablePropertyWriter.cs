using System.Reflection;
using EntitiesBT.Core;
using JetBrains.Annotations;
using Unity.Entities;

namespace EntitiesBT.Variable
{
    public interface IVariablePropertyWriter<T> where T : unmanaged
    {
        void Allocate(
            ref BlobBuilder builder
          , ref BlobVariableWriter<T> blobVariable
          , INodeDataBuilder self
          , ITreeNode<INodeDataBuilder>[] tree
        );
    }

    public abstract class VariablePropertyWriter<T> : IVariablePropertyWriter<T> where T : unmanaged
    {
        public const BindingFlags FIELD_BINDING_FLAGS = BindingFlags.Instance | BindingFlags.Public;

        public virtual void Allocate(ref BlobBuilder builder, ref BlobVariableWriter<T> blobVariable, [NotNull] INodeDataBuilder self, [NotNull] ITreeNode<INodeDataBuilder>[] tree)
        {
            blobVariable.VariableId = VariablePropertyTypeId;
            AllocateData(ref builder, ref blobVariable, self, tree);
        }
        protected virtual void AllocateData(ref BlobBuilder builder, ref BlobVariableWriter<T> blobVariable, INodeDataBuilder self, ITreeNode<INodeDataBuilder>[] tree) {}
        public virtual int VariablePropertyTypeId => 0;
    }
}
