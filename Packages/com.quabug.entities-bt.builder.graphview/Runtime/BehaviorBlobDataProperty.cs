#if UNITY_EDITOR

using System.Reflection;
using GraphExt;
using GraphExt.Editor;
using JetBrains.Annotations;
using Nuwa.Editor;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace EntitiesBT.Editor
{
    public class BehaviorBlobDataProperty : INodeProperty
    {
        public int Order => 0;
        public SerializedProperty Property { get; }

        public BehaviorBlobDataProperty(SerializedProperty property)
        {
            Property = property;
        }

        public class Factory : INodePropertyFactory
        {
            public INodeProperty Create(MemberInfo memberInfo, object nodeObj, NodeId nodeId, SerializedProperty fieldProperty = null)
            {
                return new BehaviorBlobDataProperty(fieldProperty);
            }
        }

        [UsedImplicitly]
        private class ViewFactory : SingleNodePropertyViewFactory<BehaviorBlobDataProperty>
        {
            protected override VisualElement CreateView(Node node, BehaviorBlobDataProperty property, INodePropertyViewFactory factory)
            {
                return new ImmediatePropertyField(property.Property, label: null);
            }
        }
    }
}

#endif