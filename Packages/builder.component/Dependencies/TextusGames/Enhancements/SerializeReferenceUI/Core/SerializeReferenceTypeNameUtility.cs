#if UNITY_EDITOR

using System;

/// This utility exists, because serialize reference managed reference typename returns combined string
/// and not data class that contains separate strings for assembly name and for class name (and possibly namespace name)
public static class SerializeReferenceTypeNameUtility
{
    public static Type GetRealTypeFromTypename(string stringType)
    {
        var names = GetSplitNamesFromTypename(stringType);
        var realType = Type.GetType($"{names.ClassName}, {names.AssemblyName}");
        return realType;
    }
    public static (string AssemblyName, string ClassName) GetSplitNamesFromTypename(string typename)
    {
        if (string.IsNullOrEmpty(typename))
            return ("","");
        
        var typeSplitString = typename.Split(char.Parse(" "));
        var typeClassName = typeSplitString[1];
        var typeAssemblyName = typeSplitString[0];
        return (typeAssemblyName,  typeClassName);
    }  
}

#endif