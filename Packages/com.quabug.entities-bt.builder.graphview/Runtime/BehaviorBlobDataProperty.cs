#if UNITY_EDITOR

using System.Collections.Generic;
using System.Linq;
using EntitiesBT.Variant;
using GraphExt;
using GraphExt.Editor;
using JetBrains.Annotations;
using Nuwa.Editor;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace EntitiesBT.Editor
{
    public class BehaviorBlobDataProperty : INodeProperty
    {
        public int Order => 0;
        public SerializedProperty Property { get; }
        public NodeId NodeId { get; }

        public BehaviorBlobDataProperty(SerializedProperty property, in NodeId nodeId)
        {
            Property = property;
            NodeId = nodeId;
        }

        private class View : PropertyField
        {
            private readonly SerializedProperty _property;
            private readonly SerializedProperty[] _variantProperties;

            public View(SerializedProperty property) : base(property)
            {
                name = nameof(BehaviorBlobDataProperty);
                _property = property;
                _variantProperties = FindVariantProperty().ToArray();
                this.BindProperty(property.serializedObject);
            }

            // protected override void ExecuteDefaultActionAtTarget(EventBase evt)
            // {
            //     base.ExecuteDefaultActionAtTarget(evt);
            //     if (evt is SerializedPropertyChangeEvent)
            //     {
            //         foreach (var variant in _variantProperties)
            //         {
            //             var variantPropertyField = this.Q<PropertyField>(name: variant.propertyPath);
            //             if (variantPropertyField != null) AddPortContainer(variantPropertyField, variant);
            //         }
            //     }
            //
            //     void AddPortContainer(PropertyField field, SerializedProperty variant)
            //     {
            //         var inputPortId = VariantPort.CreatePortName(variant, PortDirection.Input);
            //         field.Insert(0, new PortContainer(inputPortId));
            //         var outputPortId = VariantPort.CreatePortName(variant, PortDirection.Output);
            //         field.Add(new PortContainer(outputPortId));
            //     }
            // }

            IEnumerable<SerializedProperty> FindVariantProperty()
            {
                var child = _property.Copy();
                while (child.NextVisible(enterChildren: true))
                {
                    var managedFieldType = child.GetManagedFieldType();
                    if (typeof(IVariant).IsAssignableFrom(managedFieldType))
                        yield return child.Copy();
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