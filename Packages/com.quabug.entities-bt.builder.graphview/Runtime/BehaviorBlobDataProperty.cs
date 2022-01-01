#if UNITY_EDITOR

using System;
using System.Reflection;
using GraphExt;
using GraphExt.Editor;
using JetBrains.Annotations;
using Nuwa.Blob;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine.Assertions;
using UnityEngine.UIElements;

namespace EntitiesBT.Editor
{
    public class BehaviorBlobDataProperty : INodeProperty
    {
        public int Order => 0;

        public SerializedProperty Property;

        public class Factory : INodePropertyFactory
        {
            public INodeProperty Create(MemberInfo mi, object nodeObj, NodeId nodeId, SerializedProperty property)
            {
                Assert.IsNotNull(property);
                Assert.AreEqual(MemberTypes.Field, mi.MemberType);
                Assert.AreEqual(typeof(DynamicBlobDataBuilder), ((FieldInfo)mi).FieldType);
                return new BehaviorBlobDataProperty { Property = property.FindPropertyRelative(mi.Name) };
            }
        }

        private class View : VisualElement, ITickableElement
        {
            private SerializedProperty _property;
            private PropertyField[] _propertyViews;

            public View(SerializedProperty property)
            {
                _property = property;
            }

            public void Tick()
            {
                var builders = _property.FindPropertyRelative(nameof(DynamicBlobDataBuilder.Builders));
                var names = _property.FindPropertyRelative(nameof(DynamicBlobDataBuilder.FieldNames));
                var propertyCount = Math.Min(builders.arraySize, names.arraySize);
                Array.Resize(ref _propertyViews, propertyCount);
                for (var i = 0; i < propertyCount; i++)
                {
                    var builder = builders.GetArrayElementAtIndex(i);
                    var name = names.GetArrayElementAtIndex(i);
                    var propertyField = _propertyViews[i];
                    if (propertyField == null)
                    {
                        propertyField = new PropertyField(builder, name.stringValue);
                        propertyField.BindProperty(_property.serializedObject);
                        _propertyViews[i] = propertyField;
                        Insert(i, propertyField);
                    }

                    if (propertyField.bindingPath != builder.propertyPath)
                    {
                        propertyField.bindingPath = builder.propertyPath;
                        propertyField.label = name.stringValue;
                    }
                }
            }
        }

        [UsedImplicitly]
        private class ViewFactory : NodePropertyViewFactory<BehaviorBlobDataProperty>
        {
            protected override VisualElement Create(Node node, BehaviorBlobDataProperty property, INodePropertyViewFactory factory)
            {
                return new View(property.Property);
            }
        }
    }
}

#endif