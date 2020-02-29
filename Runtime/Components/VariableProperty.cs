using System;
using System.Reflection;
using EntitiesBT.Variable;
using UnityEngine;

namespace EntitiesBT.Components
{
    [Serializable]
    public struct VariableProperty<T> where T : struct
    {
        public ValueSource ValueSource;
        public T ConstantValue;
        
        public string ComponentTypeName;
        public string ComponentValueName;
        
        public ScriptableObject ScriptableObject;
        public string ScriptableObjectValueName;
        
        public BTNode NodeObject;
        public string NodeTypeName;
        public string NodeValueName;
    }
}
