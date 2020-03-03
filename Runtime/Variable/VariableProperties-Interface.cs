namespace EntitiesBT.Variable
{

public interface Int32Property { void Allocate(ref Unity.Entities.BlobBuilder builder, ref EntitiesBT.Variable.BlobVariable<System.Int32> blobVariable); }
public interface Int64Property { void Allocate(ref Unity.Entities.BlobBuilder builder, ref EntitiesBT.Variable.BlobVariable<System.Int64> blobVariable); }
public interface SingleProperty { void Allocate(ref Unity.Entities.BlobBuilder builder, ref EntitiesBT.Variable.BlobVariable<System.Single> blobVariable); }
public interface float2Property { void Allocate(ref Unity.Entities.BlobBuilder builder, ref EntitiesBT.Variable.BlobVariable<Unity.Mathematics.float2> blobVariable); }
public interface float3Property { void Allocate(ref Unity.Entities.BlobBuilder builder, ref EntitiesBT.Variable.BlobVariable<Unity.Mathematics.float3> blobVariable); }
public interface quaternionProperty { void Allocate(ref Unity.Entities.BlobBuilder builder, ref EntitiesBT.Variable.BlobVariable<Unity.Mathematics.quaternion> blobVariable); }

}

