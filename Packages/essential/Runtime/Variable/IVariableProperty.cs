using EntitiesBT.Core;
using Unity.Entities;

namespace EntitiesBT.Variable
{
    public interface IVariableProperty
    {
        void Allocate(
            ref BlobBuilder builder
          , ref BlobVariable blobVariable
          , INodeDataBuilder self
          , ITreeNode<INodeDataBuilder>[] tree
        );
    }

    public interface IVariablePropertyReader<T> : IVariableProperty where T : struct {}
    public interface IVariablePropertyWriter<T> : IVariableProperty where T : struct {}
}
