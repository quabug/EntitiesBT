using System;
using System.Reflection;
using EntitiesBT.Core;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Scripting;
using static EntitiesBT.Core.Utilities;

namespace EntitiesBT.Variable
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


    public static class ScriptableObjectVariableProperty
    {
        private const string _GUID = "B14A224A-7ADF-4E03-8240-60DE620FF946";
        public static int ID => GuidHashCode(_GUID);

        [Serializable]
        public class Reader<T> : IVariablePropertyReader<T> where T : unmanaged
        {
            public ScriptableObject ScriptableObject;

            [VariableScriptableObjectValue("ScriptableObject")]
            public string ScriptableObjectValueName;

            public void Allocate(ref BlobBuilder builder, ref BlobVariable blobVariable, INodeDataBuilder self, ITreeNode<INodeDataBuilder>[] tree)
            {
                blobVariable.VariableId = ID;
                var type = ScriptableObject.GetType();
                FieldInfo fieldInfo = null;
                PropertyInfo propertyInfo = null;
                if (ScriptableObject != null)
                    fieldInfo = type.GetField(ScriptableObjectValueName, BindingFlags.Instance | BindingFlags.Public);
                if (fieldInfo == null)
                    propertyInfo = type.GetProperty(ScriptableObjectValueName, BindingFlags.Instance | BindingFlags.Public);

                if ((fieldInfo == null || fieldInfo.FieldType != typeof(T))
                    && (propertyInfo == null || !propertyInfo.CanRead || propertyInfo.PropertyType != typeof(T)))
                {
                    Debug.LogError($"{ScriptableObject.name}.{ScriptableObjectValueName} is not valid");
                    throw new ArgumentException();
                }

                var value = fieldInfo?.GetValue(ScriptableObject) ?? propertyInfo?.GetValue(ScriptableObject);
                builder.Allocate(ref blobVariable, (T) value);
            }

            [Preserve, ReaderMethod(_GUID)]
            private static ref T GetDataRef<TNodeBlob, TBlackboard>(ref BlobVariable blobVariable, int index, ref TNodeBlob blob, ref TBlackboard bb)
                where TNodeBlob : struct, INodeBlob
                where TBlackboard : struct, IBlackboard
            {
                return ref blobVariable.Value<T>();
            }
        }
    }
}
