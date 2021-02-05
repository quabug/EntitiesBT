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

public interface Int32VariantReaderAndWriter : EntitiesBT.Variant.IVariantReaderAndWriter<System.Int32> { }
public class Int32NodeVariantReaderAndWriter : EntitiesBT.Variant.NodeVariant.ReaderAndWriter<System.Int32>, Int32VariantReaderAndWriter { }
public class Int32ComponentVariantReaderAndWriter : EntitiesBT.Variant.ComponentVariant.ReaderAndWriter<System.Int32>, Int32VariantReaderAndWriter { }
public class Int32LocalVariantReaderAndWriter : EntitiesBT.Variant.LocalVariant.ReaderAndWriter<System.Int32>, Int32VariantReaderAndWriter { }


[System.Serializable]
public class Int32SerializedReaderAndWriterVariant : EntitiesBT.Variant.ISerializedVariantReaderAndWriter<System.Int32>
{
    [UnityEngine.SerializeField]
    private bool _isLinked = true;
    public bool IsLinked => _isLinked;

    [UnityEngine.SerializeReference]
    [EntitiesBT.Attributes.HideIf(nameof(_isLinked), false)]
    [EntitiesBT.Attributes.SerializeReferenceButton]
    private Int32VariantReaderAndWriter _readerAndWriter;
    public EntitiesBT.Variant.IVariantReaderAndWriter<System.Int32> ReaderAndWriter => _readerAndWriter;

    [UnityEngine.SerializeReference]
    [EntitiesBT.Attributes.HideIf(nameof(_isLinked))]
    [EntitiesBT.Attributes.SerializeReferenceButton]
    private Int32VariantReader _reader;
    public EntitiesBT.Variant.IVariantReader<System.Int32> Reader => _reader;

    [UnityEngine.SerializeReference]
    [EntitiesBT.Attributes.HideIf(nameof(_isLinked))]
    [EntitiesBT.Attributes.SerializeReferenceButton]
    private Int32VariantWriter _writer;
    public EntitiesBT.Variant.IVariantWriter<System.Int32> Writer => _writer;
}
public interface Int64VariantReader : EntitiesBT.Variant.IVariantReader<System.Int64> { }
public class Int64NodeVariantReader : EntitiesBT.Variant.NodeVariant.Reader<System.Int64>, Int64VariantReader { }
public class Int64ComponentVariantReader : EntitiesBT.Variant.ComponentVariant.Reader<System.Int64>, Int64VariantReader { }
public class Int64LocalVariantReader : EntitiesBT.Variant.LocalVariant.Reader<System.Int64>, Int64VariantReader { }
public class Int64ScriptableObjectVariantReader : EntitiesBT.Variant.ScriptableObjectVariant.Reader<System.Int64>, Int64VariantReader { }

public interface Int64VariantWriter : EntitiesBT.Variant.IVariantWriter<System.Int64> { }
public class Int64NodeVariantWriter : EntitiesBT.Variant.NodeVariant.Writer<System.Int64>, Int64VariantWriter { }
public class Int64ComponentVariantWriter : EntitiesBT.Variant.ComponentVariant.Writer<System.Int64>, Int64VariantWriter { }

public interface Int64VariantReaderAndWriter : EntitiesBT.Variant.IVariantReaderAndWriter<System.Int64> { }
public class Int64NodeVariantReaderAndWriter : EntitiesBT.Variant.NodeVariant.ReaderAndWriter<System.Int64>, Int64VariantReaderAndWriter { }
public class Int64ComponentVariantReaderAndWriter : EntitiesBT.Variant.ComponentVariant.ReaderAndWriter<System.Int64>, Int64VariantReaderAndWriter { }
public class Int64LocalVariantReaderAndWriter : EntitiesBT.Variant.LocalVariant.ReaderAndWriter<System.Int64>, Int64VariantReaderAndWriter { }


[System.Serializable]
public class Int64SerializedReaderAndWriterVariant : EntitiesBT.Variant.ISerializedVariantReaderAndWriter<System.Int64>
{
    [UnityEngine.SerializeField]
    private bool _isLinked = true;
    public bool IsLinked => _isLinked;

    [UnityEngine.SerializeReference]
    [EntitiesBT.Attributes.HideIf(nameof(_isLinked), false)]
    [EntitiesBT.Attributes.SerializeReferenceButton]
    private Int64VariantReaderAndWriter _readerAndWriter;
    public EntitiesBT.Variant.IVariantReaderAndWriter<System.Int64> ReaderAndWriter => _readerAndWriter;

    [UnityEngine.SerializeReference]
    [EntitiesBT.Attributes.HideIf(nameof(_isLinked))]
    [EntitiesBT.Attributes.SerializeReferenceButton]
    private Int64VariantReader _reader;
    public EntitiesBT.Variant.IVariantReader<System.Int64> Reader => _reader;

    [UnityEngine.SerializeReference]
    [EntitiesBT.Attributes.HideIf(nameof(_isLinked))]
    [EntitiesBT.Attributes.SerializeReferenceButton]
    private Int64VariantWriter _writer;
    public EntitiesBT.Variant.IVariantWriter<System.Int64> Writer => _writer;
}
public interface SingleVariantReader : EntitiesBT.Variant.IVariantReader<System.Single> { }
public class SingleNodeVariantReader : EntitiesBT.Variant.NodeVariant.Reader<System.Single>, SingleVariantReader { }
public class SingleComponentVariantReader : EntitiesBT.Variant.ComponentVariant.Reader<System.Single>, SingleVariantReader { }
public class SingleLocalVariantReader : EntitiesBT.Variant.LocalVariant.Reader<System.Single>, SingleVariantReader { }
public class SingleScriptableObjectVariantReader : EntitiesBT.Variant.ScriptableObjectVariant.Reader<System.Single>, SingleVariantReader { }

public interface SingleVariantWriter : EntitiesBT.Variant.IVariantWriter<System.Single> { }
public class SingleNodeVariantWriter : EntitiesBT.Variant.NodeVariant.Writer<System.Single>, SingleVariantWriter { }
public class SingleComponentVariantWriter : EntitiesBT.Variant.ComponentVariant.Writer<System.Single>, SingleVariantWriter { }

public interface SingleVariantReaderAndWriter : EntitiesBT.Variant.IVariantReaderAndWriter<System.Single> { }
public class SingleNodeVariantReaderAndWriter : EntitiesBT.Variant.NodeVariant.ReaderAndWriter<System.Single>, SingleVariantReaderAndWriter { }
public class SingleComponentVariantReaderAndWriter : EntitiesBT.Variant.ComponentVariant.ReaderAndWriter<System.Single>, SingleVariantReaderAndWriter { }
public class SingleLocalVariantReaderAndWriter : EntitiesBT.Variant.LocalVariant.ReaderAndWriter<System.Single>, SingleVariantReaderAndWriter { }


[System.Serializable]
public class SingleSerializedReaderAndWriterVariant : EntitiesBT.Variant.ISerializedVariantReaderAndWriter<System.Single>
{
    [UnityEngine.SerializeField]
    private bool _isLinked = true;
    public bool IsLinked => _isLinked;

    [UnityEngine.SerializeReference]
    [EntitiesBT.Attributes.HideIf(nameof(_isLinked), false)]
    [EntitiesBT.Attributes.SerializeReferenceButton]
    private SingleVariantReaderAndWriter _readerAndWriter;
    public EntitiesBT.Variant.IVariantReaderAndWriter<System.Single> ReaderAndWriter => _readerAndWriter;

    [UnityEngine.SerializeReference]
    [EntitiesBT.Attributes.HideIf(nameof(_isLinked))]
    [EntitiesBT.Attributes.SerializeReferenceButton]
    private SingleVariantReader _reader;
    public EntitiesBT.Variant.IVariantReader<System.Single> Reader => _reader;

    [UnityEngine.SerializeReference]
    [EntitiesBT.Attributes.HideIf(nameof(_isLinked))]
    [EntitiesBT.Attributes.SerializeReferenceButton]
    private SingleVariantWriter _writer;
    public EntitiesBT.Variant.IVariantWriter<System.Single> Writer => _writer;
}
public interface float2VariantReader : EntitiesBT.Variant.IVariantReader<Unity.Mathematics.float2> { }
public class float2NodeVariantReader : EntitiesBT.Variant.NodeVariant.Reader<Unity.Mathematics.float2>, float2VariantReader { }
public class float2ComponentVariantReader : EntitiesBT.Variant.ComponentVariant.Reader<Unity.Mathematics.float2>, float2VariantReader { }
public class float2LocalVariantReader : EntitiesBT.Variant.LocalVariant.Reader<Unity.Mathematics.float2>, float2VariantReader { }
public class float2ScriptableObjectVariantReader : EntitiesBT.Variant.ScriptableObjectVariant.Reader<Unity.Mathematics.float2>, float2VariantReader { }

public interface float2VariantWriter : EntitiesBT.Variant.IVariantWriter<Unity.Mathematics.float2> { }
public class float2NodeVariantWriter : EntitiesBT.Variant.NodeVariant.Writer<Unity.Mathematics.float2>, float2VariantWriter { }
public class float2ComponentVariantWriter : EntitiesBT.Variant.ComponentVariant.Writer<Unity.Mathematics.float2>, float2VariantWriter { }

public interface float2VariantReaderAndWriter : EntitiesBT.Variant.IVariantReaderAndWriter<Unity.Mathematics.float2> { }
public class float2NodeVariantReaderAndWriter : EntitiesBT.Variant.NodeVariant.ReaderAndWriter<Unity.Mathematics.float2>, float2VariantReaderAndWriter { }
public class float2ComponentVariantReaderAndWriter : EntitiesBT.Variant.ComponentVariant.ReaderAndWriter<Unity.Mathematics.float2>, float2VariantReaderAndWriter { }
public class float2LocalVariantReaderAndWriter : EntitiesBT.Variant.LocalVariant.ReaderAndWriter<Unity.Mathematics.float2>, float2VariantReaderAndWriter { }


[System.Serializable]
public class float2SerializedReaderAndWriterVariant : EntitiesBT.Variant.ISerializedVariantReaderAndWriter<Unity.Mathematics.float2>
{
    [UnityEngine.SerializeField]
    private bool _isLinked = true;
    public bool IsLinked => _isLinked;

    [UnityEngine.SerializeReference]
    [EntitiesBT.Attributes.HideIf(nameof(_isLinked), false)]
    [EntitiesBT.Attributes.SerializeReferenceButton]
    private float2VariantReaderAndWriter _readerAndWriter;
    public EntitiesBT.Variant.IVariantReaderAndWriter<Unity.Mathematics.float2> ReaderAndWriter => _readerAndWriter;

    [UnityEngine.SerializeReference]
    [EntitiesBT.Attributes.HideIf(nameof(_isLinked))]
    [EntitiesBT.Attributes.SerializeReferenceButton]
    private float2VariantReader _reader;
    public EntitiesBT.Variant.IVariantReader<Unity.Mathematics.float2> Reader => _reader;

    [UnityEngine.SerializeReference]
    [EntitiesBT.Attributes.HideIf(nameof(_isLinked))]
    [EntitiesBT.Attributes.SerializeReferenceButton]
    private float2VariantWriter _writer;
    public EntitiesBT.Variant.IVariantWriter<Unity.Mathematics.float2> Writer => _writer;
}
public interface float3VariantReader : EntitiesBT.Variant.IVariantReader<Unity.Mathematics.float3> { }
public class float3NodeVariantReader : EntitiesBT.Variant.NodeVariant.Reader<Unity.Mathematics.float3>, float3VariantReader { }
public class float3ComponentVariantReader : EntitiesBT.Variant.ComponentVariant.Reader<Unity.Mathematics.float3>, float3VariantReader { }
public class float3LocalVariantReader : EntitiesBT.Variant.LocalVariant.Reader<Unity.Mathematics.float3>, float3VariantReader { }
public class float3ScriptableObjectVariantReader : EntitiesBT.Variant.ScriptableObjectVariant.Reader<Unity.Mathematics.float3>, float3VariantReader { }

public interface float3VariantWriter : EntitiesBT.Variant.IVariantWriter<Unity.Mathematics.float3> { }
public class float3NodeVariantWriter : EntitiesBT.Variant.NodeVariant.Writer<Unity.Mathematics.float3>, float3VariantWriter { }
public class float3ComponentVariantWriter : EntitiesBT.Variant.ComponentVariant.Writer<Unity.Mathematics.float3>, float3VariantWriter { }

public interface float3VariantReaderAndWriter : EntitiesBT.Variant.IVariantReaderAndWriter<Unity.Mathematics.float3> { }
public class float3NodeVariantReaderAndWriter : EntitiesBT.Variant.NodeVariant.ReaderAndWriter<Unity.Mathematics.float3>, float3VariantReaderAndWriter { }
public class float3ComponentVariantReaderAndWriter : EntitiesBT.Variant.ComponentVariant.ReaderAndWriter<Unity.Mathematics.float3>, float3VariantReaderAndWriter { }
public class float3LocalVariantReaderAndWriter : EntitiesBT.Variant.LocalVariant.ReaderAndWriter<Unity.Mathematics.float3>, float3VariantReaderAndWriter { }


[System.Serializable]
public class float3SerializedReaderAndWriterVariant : EntitiesBT.Variant.ISerializedVariantReaderAndWriter<Unity.Mathematics.float3>
{
    [UnityEngine.SerializeField]
    private bool _isLinked = true;
    public bool IsLinked => _isLinked;

    [UnityEngine.SerializeReference]
    [EntitiesBT.Attributes.HideIf(nameof(_isLinked), false)]
    [EntitiesBT.Attributes.SerializeReferenceButton]
    private float3VariantReaderAndWriter _readerAndWriter;
    public EntitiesBT.Variant.IVariantReaderAndWriter<Unity.Mathematics.float3> ReaderAndWriter => _readerAndWriter;

    [UnityEngine.SerializeReference]
    [EntitiesBT.Attributes.HideIf(nameof(_isLinked))]
    [EntitiesBT.Attributes.SerializeReferenceButton]
    private float3VariantReader _reader;
    public EntitiesBT.Variant.IVariantReader<Unity.Mathematics.float3> Reader => _reader;

    [UnityEngine.SerializeReference]
    [EntitiesBT.Attributes.HideIf(nameof(_isLinked))]
    [EntitiesBT.Attributes.SerializeReferenceButton]
    private float3VariantWriter _writer;
    public EntitiesBT.Variant.IVariantWriter<Unity.Mathematics.float3> Writer => _writer;
}
public interface quaternionVariantReader : EntitiesBT.Variant.IVariantReader<Unity.Mathematics.quaternion> { }
public class quaternionNodeVariantReader : EntitiesBT.Variant.NodeVariant.Reader<Unity.Mathematics.quaternion>, quaternionVariantReader { }
public class quaternionComponentVariantReader : EntitiesBT.Variant.ComponentVariant.Reader<Unity.Mathematics.quaternion>, quaternionVariantReader { }
public class quaternionLocalVariantReader : EntitiesBT.Variant.LocalVariant.Reader<Unity.Mathematics.quaternion>, quaternionVariantReader { }
public class quaternionScriptableObjectVariantReader : EntitiesBT.Variant.ScriptableObjectVariant.Reader<Unity.Mathematics.quaternion>, quaternionVariantReader { }

public interface quaternionVariantWriter : EntitiesBT.Variant.IVariantWriter<Unity.Mathematics.quaternion> { }
public class quaternionNodeVariantWriter : EntitiesBT.Variant.NodeVariant.Writer<Unity.Mathematics.quaternion>, quaternionVariantWriter { }
public class quaternionComponentVariantWriter : EntitiesBT.Variant.ComponentVariant.Writer<Unity.Mathematics.quaternion>, quaternionVariantWriter { }

public interface quaternionVariantReaderAndWriter : EntitiesBT.Variant.IVariantReaderAndWriter<Unity.Mathematics.quaternion> { }
public class quaternionNodeVariantReaderAndWriter : EntitiesBT.Variant.NodeVariant.ReaderAndWriter<Unity.Mathematics.quaternion>, quaternionVariantReaderAndWriter { }
public class quaternionComponentVariantReaderAndWriter : EntitiesBT.Variant.ComponentVariant.ReaderAndWriter<Unity.Mathematics.quaternion>, quaternionVariantReaderAndWriter { }
public class quaternionLocalVariantReaderAndWriter : EntitiesBT.Variant.LocalVariant.ReaderAndWriter<Unity.Mathematics.quaternion>, quaternionVariantReaderAndWriter { }


[System.Serializable]
public class quaternionSerializedReaderAndWriterVariant : EntitiesBT.Variant.ISerializedVariantReaderAndWriter<Unity.Mathematics.quaternion>
{
    [UnityEngine.SerializeField]
    private bool _isLinked = true;
    public bool IsLinked => _isLinked;

    [UnityEngine.SerializeReference]
    [EntitiesBT.Attributes.HideIf(nameof(_isLinked), false)]
    [EntitiesBT.Attributes.SerializeReferenceButton]
    private quaternionVariantReaderAndWriter _readerAndWriter;
    public EntitiesBT.Variant.IVariantReaderAndWriter<Unity.Mathematics.quaternion> ReaderAndWriter => _readerAndWriter;

    [UnityEngine.SerializeReference]
    [EntitiesBT.Attributes.HideIf(nameof(_isLinked))]
    [EntitiesBT.Attributes.SerializeReferenceButton]
    private quaternionVariantReader _reader;
    public EntitiesBT.Variant.IVariantReader<Unity.Mathematics.quaternion> Reader => _reader;

    [UnityEngine.SerializeReference]
    [EntitiesBT.Attributes.HideIf(nameof(_isLinked))]
    [EntitiesBT.Attributes.SerializeReferenceButton]
    private quaternionVariantWriter _writer;
    public EntitiesBT.Variant.IVariantWriter<Unity.Mathematics.quaternion> Writer => _writer;
}

}

