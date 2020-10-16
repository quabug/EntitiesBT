namespace EntitiesBT.Variable
{

// public class SingleCustomVariableProperty : EntitiesBT.Variable.CustomVariableProperty<System.Single>, SingleProperty { }

public interface INt32PropertyReader : EntitiesBT.Variable.IVariablePropertyReader<System.Int32> { }
public class Int32CustomVariablePropertyReader : EntitiesBT.Variable.CustomVariablePropertyReader<System.Int32>, INt32PropertyReader { }

public interface INt64PropertyReader : EntitiesBT.Variable.IVariablePropertyReader<System.Int64> { }
public class Int64CustomVariablePropertyReader : EntitiesBT.Variable.CustomVariablePropertyReader<System.Int64>, INt64PropertyReader { }

public interface IFloat2PropertyReader : EntitiesBT.Variable.IVariablePropertyReader<Unity.Mathematics.float2> { }
public class Float2NodeVariablePropertyReader : EntitiesBT.Variable.NodeVariablePropertyReader<Unity.Mathematics.float2>, IFloat2PropertyReader { }
public class Float2CustomVariablePropertyReader : EntitiesBT.Variable.CustomVariablePropertyReader<Unity.Mathematics.float2>, IFloat2PropertyReader { }

public interface IFloat2PropertyWriter : EntitiesBT.Variable.IVariablePropertyWriter<Unity.Mathematics.float2> { }

public interface IFloat3PropertyReader : EntitiesBT.Variable.IVariablePropertyReader<Unity.Mathematics.float3> { }
public class Float3NodeVariablePropertyReader : EntitiesBT.Variable.NodeVariablePropertyReader<Unity.Mathematics.float3>, IFloat3PropertyReader { }
public class Float3CustomVariablePropertyReader : EntitiesBT.Variable.CustomVariablePropertyReader<Unity.Mathematics.float3>, IFloat3PropertyReader { }

public interface IFloat3PropertyWriter : EntitiesBT.Variable.IVariablePropertyWriter<Unity.Mathematics.float3> { }

public interface IQuaternionPropertyReader : EntitiesBT.Variable.IVariablePropertyReader<Unity.Mathematics.quaternion> { }
public class QuaternionNodeVariablePropertyReader : EntitiesBT.Variable.NodeVariablePropertyReader<Unity.Mathematics.quaternion>, IQuaternionPropertyReader { }
public class QuaternionCustomVariablePropertyReader : EntitiesBT.Variable.CustomVariablePropertyReader<Unity.Mathematics.quaternion>, IQuaternionPropertyReader { }

public interface IQuaternionPropertyWriter : EntitiesBT.Variable.IVariablePropertyWriter<Unity.Mathematics.quaternion> { }

}

