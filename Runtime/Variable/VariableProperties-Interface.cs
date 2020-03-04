namespace EntitiesBT.Variable
{

public interface Int32Property { void Allocate(ref Unity.Entities.BlobBuilder builder, ref EntitiesBT.Variable.BlobVariable<System.Int32> blobVariable, EntitiesBT.Core.INodeDataBuilder self, EntitiesBT.Core.ITreeNode<EntitiesBT.Core.INodeDataBuilder>[] tree); }
public interface Int64Property { void Allocate(ref Unity.Entities.BlobBuilder builder, ref EntitiesBT.Variable.BlobVariable<System.Int64> blobVariable, EntitiesBT.Core.INodeDataBuilder self, EntitiesBT.Core.ITreeNode<EntitiesBT.Core.INodeDataBuilder>[] tree); }
public interface SingleProperty { void Allocate(ref Unity.Entities.BlobBuilder builder, ref EntitiesBT.Variable.BlobVariable<System.Single> blobVariable, EntitiesBT.Core.INodeDataBuilder self, EntitiesBT.Core.ITreeNode<EntitiesBT.Core.INodeDataBuilder>[] tree); }
public interface float2Property { void Allocate(ref Unity.Entities.BlobBuilder builder, ref EntitiesBT.Variable.BlobVariable<Unity.Mathematics.float2> blobVariable, EntitiesBT.Core.INodeDataBuilder self, EntitiesBT.Core.ITreeNode<EntitiesBT.Core.INodeDataBuilder>[] tree); }
public interface float3Property { void Allocate(ref Unity.Entities.BlobBuilder builder, ref EntitiesBT.Variable.BlobVariable<Unity.Mathematics.float3> blobVariable, EntitiesBT.Core.INodeDataBuilder self, EntitiesBT.Core.ITreeNode<EntitiesBT.Core.INodeDataBuilder>[] tree); }
public interface quaternionProperty { void Allocate(ref Unity.Entities.BlobBuilder builder, ref EntitiesBT.Variable.BlobVariable<Unity.Mathematics.quaternion> blobVariable, EntitiesBT.Core.INodeDataBuilder self, EntitiesBT.Core.ITreeNode<EntitiesBT.Core.INodeDataBuilder>[] tree); }

}

