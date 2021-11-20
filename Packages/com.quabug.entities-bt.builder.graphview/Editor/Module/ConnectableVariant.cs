using System;
using System.Reflection;
using Nuwa.Editor;
using UnityEditor;

namespace EntitiesBT.Editor
{
    public class ConnectableVariant
    {
        public string Id { get; }
        public Type VariantType { get; }
        internal Action<UnityEngine.Object> SetVariantNode { get; }

        public ConnectableVariant(SerializedProperty property)
        {
            Id = property.propertyPath;
            VariantType = property.GetManagedFullType();
            var fieldInfo = typeof(GraphNodeVariant.Any).GetField(nameof(GraphNodeVariant.Any.Node), BindingFlags.Instance | BindingFlags.Public);
            var variantObject = property.GetObject();
            SetVariantNode = obj => fieldInfo.SetValue(variantObject, obj);
        }
    }
}