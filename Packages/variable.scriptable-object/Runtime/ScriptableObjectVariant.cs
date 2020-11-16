using System;
using System.Reflection;
using EntitiesBT.Core;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Scripting;
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


    public static class ScriptableObjectVariant
    {
        public const string GUID = "B14A224A-7ADF-4E03-8240-60DE620FF946";

        [Serializable]
        public class Reader<T> : IVariantReader<T> where T : unmanaged
        {
            public ScriptableObject ScriptableObject;

            [VariantScriptableObjectValue("ScriptableObject")]
            public string ScriptableObjectValueName;

            public void Allocate(ref BlobBuilder builder, ref BlobVariant blobVariant, INodeDataBuilder self, ITreeNode<INodeDataBuilder>[] tree)
            {
                blobVariant.VariantId = GuidHashCode(GUID);
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
                builder.Allocate(ref blobVariant, (T) value);
            }

            [Preserve, ReaderMethod(GUID)]
            private static ref T GetDataRef<TNodeBlob, TBlackboard>(ref BlobVariant blobVariant, int index, ref TNodeBlob blob, ref TBlackboard bb)
                where TNodeBlob : struct, INodeBlob
                where TBlackboard : struct, IBlackboard
            {
                return ref blobVariant.Value<T>();
            }
        }

        public class Writer<T> : IVariantWriter<T> where T : unmanaged
        {
            public void Allocate(ref BlobBuilder builder, ref BlobVariant blobVariant, INodeDataBuilder self, ITreeNode<INodeDataBuilder>[] tree)
            {
                throw new NotImplementedException();
            }
        }
    }
}
