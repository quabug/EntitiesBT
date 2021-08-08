using System;
using System.Reflection;
using EntitiesBT.Attributes;
using EntitiesBT.Core;
using EntitiesBT.Nodes;
using Unity.Entities;
using UnityEngine;

namespace EntitiesBT.Components
{
    public class BTDynamicNode : BTNode
    {
        [SerializeReference]
        [SerializeReferenceDrawer(RenamePatter = @"^.*(\.|_|/)(\w+)(\+\w+)?$||$2", CategoryName = nameof(CategoryName))]
        [OnValueChanged(nameof(OnNodeChanged), PropertyName = nameof(ISerializableNodeData.NodeType))]
        public ISerializableNodeData NodeData;

        public bool RunOnMainThread = false;

        protected override Type NodeType => NodeData?.NodeType ?? typeof(ZeroNode);

        protected override unsafe void Build(void* dataPtr, BlobBuilder blobBuilder, ITreeNode<INodeDataBuilder>[] builders)
        {
            NodeData?.Build(dataPtr, blobBuilder, Self, builders);
        }

        protected override INodeDataBuilder SelfImpl => RunOnMainThread ? new BTVirtualDecorator<RunOnMainThreadNode>(this) : (INodeDataBuilder) this;

        private void OnNodeChanged()
        {
            name = NodeData?.NodeType?.Name ?? "[Null]";
        }

        private string CategoryName(Type type)
        {
            for (;;)
            {
                if (type == null) return "";
                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(SerializableNodeData<>))
                    return type.GetGenericArguments()[0].GetCustomAttribute<BehaviorNodeAttribute>()?.Type.ToString() ?? "";
                type = type.BaseType;
            }
        }
    }
}