using System;
using System.Reflection;
using Blob;
using EntitiesBT.Core;
using Unity.Entities;
using UnityEngine;
using static EntitiesBT.Core.Utilities;

namespace EntitiesBT.Variant
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class VariantScriptableObjectValueAttribute : PropertyAttribute
    {
        public string ScriptableObjectFieldName;

        public VariantScriptableObjectValueAttribute(string scriptableObjectFieldName)
        {
            ScriptableObjectFieldName = scriptableObjectFieldName;
        }
    }

    [VariantClass(GUID)]
    public static class ScriptableObjectVariant
    {
        public const string GUID = "B14A224A-7ADF-4E03-8240-60DE620FF946";

        [Serializable]
        public class Reader<T> : IVariantReader<T> where T : unmanaged
        {
            public ScriptableObject ScriptableObject;

            [VariantScriptableObjectValue(nameof(ScriptableObject))]
            public string ScriptableObjectValueName;

            public void Allocate(BlobVariantStream stream)
            {
                stream.SetVariantId(GuidHashCode(GUID));
                stream.SetVariantValue((T)PreviewValue);
            }

            public object PreviewValue
            {
                get
                {
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

                    return fieldInfo?.GetValue(ScriptableObject) ?? propertyInfo?.GetValue(ScriptableObject);
                }
            }
        }

        [RefReaderMethod]
        private static ref T GetDataRef<T, TNodeBlob, TBlackboard>(ref BlobVariant blobVariant, int index, ref TNodeBlob blob, ref TBlackboard bb)
            where T : unmanaged
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
            return ref blobVariant.As<T>();
        }

    }
}
