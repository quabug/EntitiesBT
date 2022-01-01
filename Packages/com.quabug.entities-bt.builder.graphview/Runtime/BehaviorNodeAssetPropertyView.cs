#if UNITY_EDITOR

using System.Linq;
using System.Reflection;
using EntitiesBT.Components;
using GraphExt;
using GraphExt.Editor;
using Nuwa.Blob;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine.Assertions;
using UnityEngine.UIElements;

namespace EntitiesBT.Editor
{
    public class BehaviorNodeAssetProperty : INodeProperty
    {
        public int Order => 0;

        public SerializedProperty[] Builders;
        public string[] Names;

        public class Factory : INodePropertyFactory
        {
            public INodeProperty Create(MemberInfo mi, object nodeObj, NodeId nodeId, SerializedProperty property)
            {
                Assert.IsNotNull(property);
                Assert.AreEqual(MemberTypes.Field, mi.MemberType);
                Assert.AreEqual(typeof(NodeAsset), ((FieldInfo)mi).FieldType);
                var blobProperty = property.FindPropertyRelative(mi.Name).FindPropertyRelative(nameof(NodeAsset.Builder));
                var builders = blobProperty.FindPropertyRelative(nameof(DynamicBlobDataBuilder.Builders));
                var names = blobProperty.FindPropertyRelative(nameof(DynamicBlobDataBuilder.FieldNames));
                return new BehaviorNodeAssetProperty
                {
                    Builders = Enumerable.Range(0, builders.arraySize).Select(index => builders.GetArrayElementAtIndex(index)).ToArray(),
                    Names = Enumerable.Range(0, names.arraySize).Select(index => names.GetArrayElementAtIndex(index).stringValue).ToArray()
                };
            }
        }

        public class ViewFactory : NodePropertyViewFactory<BehaviorNodeAssetProperty>
        {
            protected override VisualElement Create(Node node, BehaviorNodeAssetProperty property, INodePropertyViewFactory factory)
            {
                var element = new VisualElement();
                foreach (var (name, builder) in property.Names.Zip(property.Builders, (name, builder) => (name, builder)))
                {
                    var propertyField = new PropertyField(builder);
                    propertyField.BindProperty(builder.serializedObject);
                    element.Add(propertyField);
                }
                return element;
            }
        }
    }
}

#endif