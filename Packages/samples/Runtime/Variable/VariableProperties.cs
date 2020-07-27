namespace EntitiesBT.Variable
{

public class SingleCustomVariableProperty : EntitiesBT.Variable.CustomVariableProperty<System.Single>, SingleProperty { }

public interface Int32Property : EntitiesBT.Variable.IVariableProperty<System.Int32> { }
public class Int32CustomVariableProperty : EntitiesBT.Variable.CustomVariableProperty<System.Int32>, Int32Property { }

public interface Int64Property : EntitiesBT.Variable.IVariableProperty<System.Int64> { }
public class Int64CustomVariableProperty : EntitiesBT.Variable.CustomVariableProperty<System.Int64>, Int64Property { }

public interface float2Property : EntitiesBT.Variable.IVariableProperty<Unity.Mathematics.float2> { }
public class float2NodeVariableProperty : EntitiesBT.Variable.NodeVariableProperty<Unity.Mathematics.float2>, float2Property { }
public class float2CustomVariableProperty : EntitiesBT.Variable.CustomVariableProperty<Unity.Mathematics.float2>, float2Property { }

public interface float3Property : EntitiesBT.Variable.IVariableProperty<Unity.Mathematics.float3> { }
public class float3NodeVariableProperty : EntitiesBT.Variable.NodeVariableProperty<Unity.Mathematics.float3>, float3Property { }
public class float3CustomVariableProperty : EntitiesBT.Variable.CustomVariableProperty<Unity.Mathematics.float3>, float3Property { }

public interface quaternionProperty : EntitiesBT.Variable.IVariableProperty<Unity.Mathematics.quaternion> { }
public class quaternionNodeVariableProperty : EntitiesBT.Variable.NodeVariableProperty<Unity.Mathematics.quaternion>, quaternionProperty { }
public class quaternionCustomVariableProperty : EntitiesBT.Variable.CustomVariableProperty<Unity.Mathematics.quaternion>, quaternionProperty { }

}

