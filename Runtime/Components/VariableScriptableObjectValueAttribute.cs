using System;
using UnityEngine;

namespace EntitiesBT.Components
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class VariableScriptableObjectValueAttribute : PropertyAttribute
    {
        public string ScriptableObjectFieldName;

        public VariableScriptableObjectValueAttribute(string scriptableObjectFieldName)
        {
            ScriptableObjectFieldName = scriptableObjectFieldName;
        }
    }
}
