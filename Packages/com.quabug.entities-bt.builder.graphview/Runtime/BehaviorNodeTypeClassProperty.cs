using System;
using System.Reflection;
using EntitiesBT.Core;
using GraphExt;
using GraphExt.Editor;
using JetBrains.Annotations;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.Assertions;
using UnityEngine.UIElements;

namespace EntitiesBT.Editor
{
    public class BehaviorNodeTypeClassProperty : INodeProperty
    {
        public int Order { get; }
        public Type BehaviorNodeType;

        public class Factory : INodePropertyFactory
        {
            public INodeProperty Create(MemberInfo mi, object nodeObj, NodeId nodeId, SerializedProperty nodeSerializedProperty = null)
            {
                Assert.IsNotNull(mi);
                Assert.IsNotNull(nodeObj);
                return new BehaviorNodeTypeClassProperty{ BehaviorNodeType = mi.GetValue<Type>(nodeObj) };
            }
        }

        [UsedImplicitly]
        private class ViewFactory : NodePropertyViewFactory<BehaviorNodeTypeClassProperty>
        {
            protected override VisualElement Create(Node node, BehaviorNodeTypeClassProperty property, INodePropertyViewFactory factory)
            {
                var behaviorNodeAttribute = property.BehaviorNodeType.GetCustomAttribute<BehaviorNodeAttribute>();
                if (behaviorNodeAttribute != null) node.AddToClassList(behaviorNodeAttribute.Type.ToString());
                return null;
            }
        }
    }
}