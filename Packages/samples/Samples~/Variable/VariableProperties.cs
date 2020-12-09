namespace EntitiesBT.Sample
{

public interface Int32VariantReader : EntitiesBT.Variant.IVariantReader<System.Int32> { }
public class Int32NodeVariantReader : EntitiesBT.Variant.NodeVariant.Reader<System.Int32>, Int32VariantReader { }
public class Int32ComponentVariantReader : EntitiesBT.Variant.ComponentVariant.Reader<System.Int32>, Int32VariantReader { }
public class Int32LocalVariantReader : EntitiesBT.Variant.LocalVariant.Reader<System.Int32>, Int32VariantReader { }
public class Int32ScriptableObjectVariantReader : EntitiesBT.Variant.ScriptableObjectVariant.Reader<System.Int32>, Int32VariantReader { }

public interface Int32VariantWriter : EntitiesBT.Variant.IVariantWriter<System.Int32> { }
public class Int32NodeVariantWriter : EntitiesBT.Variant.NodeVariant.Writer<System.Int32>, Int32VariantWriter { }
public class Int32ComponentVariantWriter : EntitiesBT.Variant.ComponentVariant.Writer<System.Int32>, Int32VariantWriter { }
// public class Int32LocalVariantWriter : EntitiesBT.Variant.LocalVariant.Writer<System.Int32>, Int32VariantWriter { }
// public class Int32ScriptableObjectVariantWriter : EntitiesBT.Variant.ScriptableObjectVariant.Writer<System.Int32>, Int32VariantWriter { }

public interface Int64VariantReader : EntitiesBT.Variant.IVariantReader<System.Int64> { }
public class Int64NodeVariantReader : EntitiesBT.Variant.NodeVariant.Reader<System.Int64>, Int64VariantReader { }
public class Int64ComponentVariantReader : EntitiesBT.Variant.ComponentVariant.Reader<System.Int64>, Int64VariantReader { }
public class Int64LocalVariantReader : EntitiesBT.Variant.LocalVariant.Reader<System.Int64>, Int64VariantReader { }
public class Int64ScriptableObjectVariantReader : EntitiesBT.Variant.ScriptableObjectVariant.Reader<System.Int64>, Int64VariantReader { }

public interface Int64VariantWriter : EntitiesBT.Variant.IVariantWriter<System.Int64> { }
public class Int64NodeVariantWriter : EntitiesBT.Variant.NodeVariant.Writer<System.Int64>, Int64VariantWriter { }
public class Int64ComponentVariantWriter : EntitiesBT.Variant.ComponentVariant.Writer<System.Int64>, Int64VariantWriter { }
// public class Int64LocalVariantWriter : EntitiesBT.Variant.LocalVariant.Writer<System.Int64>, Int64VariantWriter { }
// public class Int64ScriptableObjectVariantWriter : EntitiesBT.Variant.ScriptableObjectVariant.Writer<System.Int64>, Int64VariantWriter { }

public interface SingleVariantReader : EntitiesBT.Variant.IVariantReader<System.Single> { }
public class SingleNodeVariantReader : EntitiesBT.Variant.NodeVariant.Reader<System.Single>, SingleVariantReader { }
public class SingleComponentVariantReader : EntitiesBT.Variant.ComponentVariant.Reader<System.Single>, SingleVariantReader { }
public class SingleLocalVariantReader : EntitiesBT.Variant.LocalVariant.Reader<System.Single>, SingleVariantReader { }
public class SingleScriptableObjectVariantReader : EntitiesBT.Variant.ScriptableObjectVariant.Reader<System.Single>, SingleVariantReader { }

public interface SingleVariantWriter : EntitiesBT.Variant.IVariantWriter<System.Single> { }
public class SingleNodeVariantWriter : EntitiesBT.Variant.NodeVariant.Writer<System.Single>, SingleVariantWriter { }
public class SingleComponentVariantWriter : EntitiesBT.Variant.ComponentVariant.Writer<System.Single>, SingleVariantWriter { }
// public class SingleLocalVariantWriter : EntitiesBT.Variant.LocalVariant.Writer<System.Single>, SingleVariantWriter { }
// public class SingleScriptableObjectVariantWriter : EntitiesBT.Variant.ScriptableObjectVariant.Writer<System.Single>, SingleVariantWriter { }

public interface float2VariantReader : EntitiesBT.Variant.IVariantReader<Unity.Mathematics.float2> { }
public class float2NodeVariantReader : EntitiesBT.Variant.NodeVariant.Reader<Unity.Mathematics.float2>, float2VariantReader { }
public class float2ComponentVariantReader : EntitiesBT.Variant.ComponentVariant.Reader<Unity.Mathematics.float2>, float2VariantReader { }
public class float2LocalVariantReader : EntitiesBT.Variant.LocalVariant.Reader<Unity.Mathematics.float2>, float2VariantReader { }
public class float2ScriptableObjectVariantReader : EntitiesBT.Variant.ScriptableObjectVariant.Reader<Unity.Mathematics.float2>, float2VariantReader { }

public interface float2VariantWriter : EntitiesBT.Variant.IVariantWriter<Unity.Mathematics.float2> { }
public class float2NodeVariantWriter : EntitiesBT.Variant.NodeVariant.Writer<Unity.Mathematics.float2>, float2VariantWriter { }
public class float2ComponentVariantWriter : EntitiesBT.Variant.ComponentVariant.Writer<Unity.Mathematics.float2>, float2VariantWriter { }
// public class float2LocalVariantWriter : EntitiesBT.Variant.LocalVariant.Writer<Unity.Mathematics.float2>, float2VariantWriter { }
// public class float2ScriptableObjectVariantWriter : EntitiesBT.Variant.ScriptableObjectVariant.Writer<Unity.Mathematics.float2>, float2VariantWriter { }

public interface float3VariantReader : EntitiesBT.Variant.IVariantReader<Unity.Mathematics.float3> { }
public class float3NodeVariantReader : EntitiesBT.Variant.NodeVariant.Reader<Unity.Mathematics.float3>, float3VariantReader { }
public class float3ComponentVariantReader : EntitiesBT.Variant.ComponentVariant.Reader<Unity.Mathematics.float3>, float3VariantReader { }
public class float3LocalVariantReader : EntitiesBT.Variant.LocalVariant.Reader<Unity.Mathematics.float3>, float3VariantReader { }
public class float3ScriptableObjectVariantReader : EntitiesBT.Variant.ScriptableObjectVariant.Reader<Unity.Mathematics.float3>, float3VariantReader { }

public interface float3VariantWriter : EntitiesBT.Variant.IVariantWriter<Unity.Mathematics.float3> { }
public class float3NodeVariantWriter : EntitiesBT.Variant.NodeVariant.Writer<Unity.Mathematics.float3>, float3VariantWriter { }
public class float3ComponentVariantWriter : EntitiesBT.Variant.ComponentVariant.Writer<Unity.Mathematics.float3>, float3VariantWriter { }
// public class float3LocalVariantWriter : EntitiesBT.Variant.LocalVariant.Writer<Unity.Mathematics.float3>, float3VariantWriter { }
// public class float3ScriptableObjectVariantWriter : EntitiesBT.Variant.ScriptableObjectVariant.Writer<Unity.Mathematics.float3>, float3VariantWriter { }

public interface quaternionVariantReader : EntitiesBT.Variant.IVariantReader<Unity.Mathematics.quaternion> { }
public class quaternionNodeVariantReader : EntitiesBT.Variant.NodeVariant.Reader<Unity.Mathematics.quaternion>, quaternionVariantReader { }
public class quaternionComponentVariantReader : EntitiesBT.Variant.ComponentVariant.Reader<Unity.Mathematics.quaternion>, quaternionVariantReader { }
public class quaternionLocalVariantReader : EntitiesBT.Variant.LocalVariant.Reader<Unity.Mathematics.quaternion>, quaternionVariantReader { }
public class quaternionScriptableObjectVariantReader : EntitiesBT.Variant.ScriptableObjectVariant.Reader<Unity.Mathematics.quaternion>, quaternionVariantReader { }

public interface quaternionVariantWriter : EntitiesBT.Variant.IVariantWriter<Unity.Mathematics.quaternion> { }
public class quaternionNodeVariantWriter : EntitiesBT.Variant.NodeVariant.Writer<Unity.Mathematics.quaternion>, quaternionVariantWriter { }
public class quaternionComponentVariantWriter : EntitiesBT.Variant.ComponentVariant.Writer<Unity.Mathematics.quaternion>, quaternionVariantWriter { }
// public class quaternionLocalVariantWriter : EntitiesBT.Variant.LocalVariant.Writer<Unity.Mathematics.quaternion>, quaternionVariantWriter { }
// public class quaternionScriptableObjectVariantWriter : EntitiesBT.Variant.ScriptableObjectVariant.Writer<Unity.Mathematics.quaternion>, quaternionVariantWriter { }


}

