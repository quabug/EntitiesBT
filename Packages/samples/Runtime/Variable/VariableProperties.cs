namespace EntitiesBT.Variable
{

// public class SingleLocalVariableProperty : EntitiesBT.Variable.LocalVariableProperty<System.Single>, SingleProperty { }

public interface Int32PropertyReader : EntitiesBT.Variable.IVariablePropertyReader<System.Int32> { }
public class Int32LocalVariablePropertyReader : EntitiesBT.Variable.LocalVariableProperty.Reader<System.Int32>, Int32PropertyReader { }

public interface Int64PropertyReader : EntitiesBT.Variable.IVariablePropertyReader<System.Int64> { }
public class Int64LocalVariablePropertyReader : EntitiesBT.Variable.LocalVariableProperty.Reader<System.Int64>, Int64PropertyReader { }

public interface IFloat2PropertyReader : EntitiesBT.Variable.IVariablePropertyReader<Unity.Mathematics.float2> { }
public class Float2NodeVariablePropertyReader : EntitiesBT.Variable.NodeVariablePropertyReader<Unity.Mathematics.float2>, IFloat2PropertyReader { }
public class Float2LocalVariablePropertyReader : EntitiesBT.Variable.LocalVariableProperty.Reader<Unity.Mathematics.float2>, IFloat2PropertyReader { }

public interface IFloat2PropertyWriter : EntitiesBT.Variable.IVariablePropertyWriter<Unity.Mathematics.float2> { }

public interface IFloat3PropertyReader : EntitiesBT.Variable.IVariablePropertyReader<Unity.Mathematics.float3> { }
public class Float3NodeVariablePropertyReader : EntitiesBT.Variable.NodeVariablePropertyReader<Unity.Mathematics.float3>, IFloat3PropertyReader { }
public class Float3LocalVariablePropertyReader : EntitiesBT.Variable.LocalVariableProperty.Reader<Unity.Mathematics.float3>, IFloat3PropertyReader { }

public interface IFloat3PropertyWriter : EntitiesBT.Variable.IVariablePropertyWriter<Unity.Mathematics.float3> { }

public interface IQuaternionPropertyReader : EntitiesBT.Variable.IVariablePropertyReader<Unity.Mathematics.quaternion> { }
public class QuaternionNodeVariablePropertyReader : EntitiesBT.Variable.NodeVariablePropertyReader<Unity.Mathematics.quaternion>, IQuaternionPropertyReader { }
public class QuaternionLocalVariablePropertyReader : EntitiesBT.Variable.LocalVariableProperty.Reader<Unity.Mathematics.quaternion>, IQuaternionPropertyReader { }

public interface IQuaternionPropertyWriter : EntitiesBT.Variable.IVariablePropertyWriter<Unity.Mathematics.quaternion> { }

}

