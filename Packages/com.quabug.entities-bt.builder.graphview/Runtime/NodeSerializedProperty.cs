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
    public class NodeSerializedProperty : INodeProperty
    {
        public int Order => 0;
        public SerializedProperty Property { get; }

        public NodeSerializedProperty(SerializedProperty property)
        {
            Property = property;
        }

        public class Factory : INodePropertyFactory
        {
            public INodeProperty Create(MemberInfo memberInfo, object nodeObj, NodeId nodeId, SerializedProperty fieldProperty = null)
            {
                return new NodeSerializedProperty(fieldProperty);
            }
        }

        [UsedImplicitly]
        private class ViewFactory : SingleNodePropertyViewFactory<NodeSerializedProperty>
        {
            protected override VisualElement CreateView(Node node, NodeSerializedProperty property, INodePropertyViewFactory factory)
            {
                return new ImmediatePropertyField(property.Property, label: null);
            }
        }
    }
}

#endif