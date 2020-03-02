namespace EntitiesBT.Variable.Sample
{


public interface Int32Property
{
    void Allocate(ref Unity.Entities.BlobBuilder builder, ref EntitiesBT.Variable.BlobVariable<System.Int32> blobVariable);
}


public class Int32ComponentVariableProperty : EntitiesBT.Variable.ComponentVariableProperty<System.Int32>, Int32Property
{
}


public class Int32CustomVariableProperty : EntitiesBT.Variable.CustomVariableProperty<System.Int32>, Int32Property
{
}


public class Int32VariableProperty : EntitiesBT.Variable.VariableProperty<System.Int32>, Int32Property
{
}


public class Int32ScriptableObjectVariableProperty : EntitiesBT.Components.ScriptableObjectVariableProperty<System.Int32>, Int32Property
{
}


public interface Int64Property
{
    void Allocate(ref Unity.Entities.BlobBuilder builder, ref EntitiesBT.Variable.BlobVariable<System.Int64> blobVariable);
}


public class Int64ComponentVariableProperty : EntitiesBT.Variable.ComponentVariableProperty<System.Int64>, Int64Property
{
}


public class Int64CustomVariableProperty : EntitiesBT.Variable.CustomVariableProperty<System.Int64>, Int64Property
{
}


public class Int64VariableProperty : EntitiesBT.Variable.VariableProperty<System.Int64>, Int64Property
{
}


public class Int64ScriptableObjectVariableProperty : EntitiesBT.Components.ScriptableObjectVariableProperty<System.Int64>, Int64Property
{
}


public interface SingleProperty
{
    void Allocate(ref Unity.Entities.BlobBuilder builder, ref EntitiesBT.Variable.BlobVariable<System.Single> blobVariable);
}


public class SingleComponentVariableProperty : EntitiesBT.Variable.ComponentVariableProperty<System.Single>, SingleProperty
{
}


public class SingleCustomVariableProperty : EntitiesBT.Variable.CustomVariableProperty<System.Single>, SingleProperty
{
}


public class SingleVariableProperty : EntitiesBT.Variable.VariableProperty<System.Single>, SingleProperty
{
}


public class SingleScriptableObjectVariableProperty : EntitiesBT.Components.ScriptableObjectVariableProperty<System.Single>, SingleProperty
{
}


}

