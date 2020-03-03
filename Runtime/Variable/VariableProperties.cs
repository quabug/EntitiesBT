namespace EntitiesBT.Variable
{

public interface Int32Property { void Allocate(ref Unity.Entities.BlobBuilder builder, ref EntitiesBT.Variable.BlobVariable<System.Int32> blobVariable); }
public class Int32ScriptableObjectVariableProperty : EntitiesBT.Components.ScriptableObjectVariableProperty<System.Int32>, Int32Property { }
public class Int32ComponentVariableProperty : EntitiesBT.Variable.ComponentVariableProperty<System.Int32>, Int32Property { }
public class Int32CustomVariableProperty : EntitiesBT.Variable.CustomVariableProperty<System.Int32>, Int32Property { }

public interface Int64Property { void Allocate(ref Unity.Entities.BlobBuilder builder, ref EntitiesBT.Variable.BlobVariable<System.Int64> blobVariable); }
public class Int64ScriptableObjectVariableProperty : EntitiesBT.Components.ScriptableObjectVariableProperty<System.Int64>, Int64Property { }
public class Int64ComponentVariableProperty : EntitiesBT.Variable.ComponentVariableProperty<System.Int64>, Int64Property { }
public class Int64CustomVariableProperty : EntitiesBT.Variable.CustomVariableProperty<System.Int64>, Int64Property { }

public interface SingleProperty { void Allocate(ref Unity.Entities.BlobBuilder builder, ref EntitiesBT.Variable.BlobVariable<System.Single> blobVariable); }
public class SingleScriptableObjectVariableProperty : EntitiesBT.Components.ScriptableObjectVariableProperty<System.Single>, SingleProperty { }
public class SingleComponentVariableProperty : EntitiesBT.Variable.ComponentVariableProperty<System.Single>, SingleProperty { }
public class SingleCustomVariableProperty : EntitiesBT.Variable.CustomVariableProperty<System.Single>, SingleProperty { }

public interface float2Property { void Allocate(ref Unity.Entities.BlobBuilder builder, ref EntitiesBT.Variable.BlobVariable<Unity.Mathematics.float2> blobVariable); }
public class float2ScriptableObjectVariableProperty : EntitiesBT.Components.ScriptableObjectVariableProperty<Unity.Mathematics.float2>, float2Property { }
public class float2ComponentVariableProperty : EntitiesBT.Variable.ComponentVariableProperty<Unity.Mathematics.float2>, float2Property { }
public class float2CustomVariableProperty : EntitiesBT.Variable.CustomVariableProperty<Unity.Mathematics.float2>, float2Property { }

public interface float3Property { void Allocate(ref Unity.Entities.BlobBuilder builder, ref EntitiesBT.Variable.BlobVariable<Unity.Mathematics.float3> blobVariable); }
public class float3ScriptableObjectVariableProperty : EntitiesBT.Components.ScriptableObjectVariableProperty<Unity.Mathematics.float3>, float3Property { }
public class float3ComponentVariableProperty : EntitiesBT.Variable.ComponentVariableProperty<Unity.Mathematics.float3>, float3Property { }
public class float3CustomVariableProperty : EntitiesBT.Variable.CustomVariableProperty<Unity.Mathematics.float3>, float3Property { }

public interface quaternionProperty { void Allocate(ref Unity.Entities.BlobBuilder builder, ref EntitiesBT.Variable.BlobVariable<Unity.Mathematics.quaternion> blobVariable); }
public class quaternionScriptableObjectVariableProperty : EntitiesBT.Components.ScriptableObjectVariableProperty<Unity.Mathematics.quaternion>, quaternionProperty { }
public class quaternionComponentVariableProperty : EntitiesBT.Variable.ComponentVariableProperty<Unity.Mathematics.quaternion>, quaternionProperty { }
public class quaternionCustomVariableProperty : EntitiesBT.Variable.CustomVariableProperty<Unity.Mathematics.quaternion>, quaternionProperty { }


}

