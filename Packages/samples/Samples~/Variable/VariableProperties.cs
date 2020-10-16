namespace EntitiesBT.Sample
{

public interface INt32PropertyReader : EntitiesBT.Variable.IVariablePropertyReader<System.Int32> { }
public class Int32ScriptableObjectVariablePropertyReader : EntitiesBT.Variable.ScriptableObjectVariablePropertyReader<System.Int32>, INt32PropertyReader { }
public class Int32NodeVariablePropertyReader : EntitiesBT.Variable.NodeVariablePropertyReader<System.Int32>, INt32PropertyReader { }
public class Int32ComponentVariablePropertyReader : EntitiesBT.Variable.ComponentVariablePropertyReader<System.Int32>, INt32PropertyReader { }
public class Int32CustomVariablePropertyReader : EntitiesBT.Variable.CustomVariablePropertyReader<System.Int32>, INt32PropertyReader { }

public interface INt64PropertyReader : EntitiesBT.Variable.IVariablePropertyReader<System.Int64> { }
public class Int64ScriptableObjectVariablePropertyReader : EntitiesBT.Variable.ScriptableObjectVariablePropertyReader<System.Int64>, INt64PropertyReader { }
public class Int64NodeVariablePropertyReader : EntitiesBT.Variable.NodeVariablePropertyReader<System.Int64>, INt64PropertyReader { }
public class Int64ComponentVariablePropertyReader : EntitiesBT.Variable.ComponentVariablePropertyReader<System.Int64>, INt64PropertyReader { }
public class Int64CustomVariablePropertyReader : EntitiesBT.Variable.CustomVariablePropertyReader<System.Int64>, INt64PropertyReader { }

public interface ISinglePropertyReader : EntitiesBT.Variable.IVariablePropertyReader<System.Single> { }
public class SingleScriptableObjectVariablePropertyReader : EntitiesBT.Variable.ScriptableObjectVariablePropertyReader<System.Single>, ISinglePropertyReader { }
public class SingleNodeVariablePropertyReader : EntitiesBT.Variable.NodeVariablePropertyReader<System.Single>, ISinglePropertyReader { }
public class SingleComponentVariablePropertyReader : EntitiesBT.Variable.ComponentVariablePropertyReader<System.Single>, ISinglePropertyReader { }
public class SingleCustomVariablePropertyReader : EntitiesBT.Variable.CustomVariablePropertyReader<System.Single>, ISinglePropertyReader { }

public interface IFloat2PropertyReader : EntitiesBT.Variable.IVariablePropertyReader<Unity.Mathematics.float2> { }
public class Float2ScriptableObjectVariablePropertyReader : EntitiesBT.Variable.ScriptableObjectVariablePropertyReader<Unity.Mathematics.float2>, IFloat2PropertyReader { }
public class Float2NodeVariablePropertyReader : EntitiesBT.Variable.NodeVariablePropertyReader<Unity.Mathematics.float2>, IFloat2PropertyReader { }
public class Float2ComponentVariablePropertyReader : EntitiesBT.Variable.ComponentVariablePropertyReader<Unity.Mathematics.float2>, IFloat2PropertyReader { }
public class Float2CustomVariablePropertyReader : EntitiesBT.Variable.CustomVariablePropertyReader<Unity.Mathematics.float2>, IFloat2PropertyReader { }

public interface IFloat3PropertyReader : EntitiesBT.Variable.IVariablePropertyReader<Unity.Mathematics.float3> { }
public class Float3ScriptableObjectVariablePropertyReader : EntitiesBT.Variable.ScriptableObjectVariablePropertyReader<Unity.Mathematics.float3>, IFloat3PropertyReader { }
public class Float3NodeVariablePropertyReader : EntitiesBT.Variable.NodeVariablePropertyReader<Unity.Mathematics.float3>, IFloat3PropertyReader { }
public class Float3ComponentVariablePropertyReader : EntitiesBT.Variable.ComponentVariablePropertyReader<Unity.Mathematics.float3>, IFloat3PropertyReader { }
public class Float3CustomVariablePropertyReader : EntitiesBT.Variable.CustomVariablePropertyReader<Unity.Mathematics.float3>, IFloat3PropertyReader { }

public interface IQuaternionPropertyReader : EntitiesBT.Variable.IVariablePropertyReader<Unity.Mathematics.quaternion> { }
public class QuaternionScriptableObjectVariablePropertyReader : EntitiesBT.Variable.ScriptableObjectVariablePropertyReader<Unity.Mathematics.quaternion>, IQuaternionPropertyReader { }
public class QuaternionNodeVariablePropertyReader : EntitiesBT.Variable.NodeVariablePropertyReader<Unity.Mathematics.quaternion>, IQuaternionPropertyReader { }
public class QuaternionComponentVariablePropertyReader : EntitiesBT.Variable.ComponentVariablePropertyReader<Unity.Mathematics.quaternion>, IQuaternionPropertyReader { }
public class QuaternionCustomVariablePropertyReader : EntitiesBT.Variable.CustomVariablePropertyReader<Unity.Mathematics.quaternion>, IQuaternionPropertyReader { }


}

