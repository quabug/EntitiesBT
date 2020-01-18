using System;
using System.Collections.Generic;
using System.IO;
using EntitiesBT.Core;
using EntitiesBT.Entities;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace EntitiesBT.Components
{
    [DisallowMultipleComponent, ExecuteInEditMode]
    public abstract class BTNode : MonoBehaviour, INodeDataBuilder
    {
        public abstract BehaviorNodeType NodeType { get; }
        public abstract int NodeId { get; }
        public abstract int Size { get; }
        public abstract unsafe void Build(void* dataPtr, ITreeNode<INodeDataBuilder>[] builders);
        public virtual IEnumerable<INodeDataBuilder> Children => this.Children();
        public virtual INodeDataBuilder Self => this;

        protected virtual void Reset() => name = GetType().Name;

        protected virtual void OnTransformChildrenChanged()
        {
            int maxChildCount;
            switch (NodeType)
            {
            case BehaviorNodeType.Composite:
                maxChildCount = int.MaxValue;
                break;
            case BehaviorNodeType.Decorate:
                maxChildCount = 1;
                break;
            case BehaviorNodeType.Action:
                maxChildCount = 0;
                break;
            default:
                throw new ArgumentOutOfRangeException();
            }
            
            var childCount = transform.childCount;
            if (childCount > maxChildCount)
            {
                Debug.LogError($"{NodeType} node {name} is not allowed to have more than {maxChildCount} children", gameObject);
                for (var i = childCount - 1; i >= maxChildCount; i--) DestroyImmediate(transform.GetChild(i).gameObject);
            }
        }

        protected virtual void OnValidate() {}

#if UNITY_EDITOR
        [ContextMenu("Save to file")]
        public void SaveToFile()
        {
            var path = UnityEditor.EditorUtility.SaveFilePanel("save path", Application.dataPath, "behavior-tree", "bytes");
            if (string.IsNullOrEmpty(path))
                return;

            using (var file = new FileStream(path, FileMode.OpenOrCreate))
                this.SaveToStream(file);
            
            UnityEditor.AssetDatabase.Refresh();
        }
#endif
    }
    
    public abstract class BTNode<T> : BTNode where T : struct, INodeData
    {
        public override BehaviorNodeType NodeType => typeof(T).GetBehaviorNodeAttribute().Type;
        public override int NodeId => typeof(T).GetBehaviorNodeAttribute().Id;
        public override int Size => UnsafeUtility.SizeOf<T>();
        public override unsafe void Build(void* dataPtr, ITreeNode<INodeDataBuilder>[] builders) =>
            Build(ref UnsafeUtilityEx.AsRef<T>(dataPtr), builders);
        protected virtual void Build(ref T data, ITreeNode<INodeDataBuilder>[] builders) {}
    }
    
    public abstract class BTVirtualNode<T> : INodeDataBuilder where T : struct, INodeData
    {
        public virtual BehaviorNodeType NodeType => typeof(T).GetBehaviorNodeAttribute().Type;
        public virtual int NodeId => typeof(T).GetBehaviorNodeAttribute().Id;
        public virtual int Size => UnsafeUtility.SizeOf<T>();
        public virtual unsafe void Build(void* dataPtr, ITreeNode<INodeDataBuilder>[] builders) =>
            Build(ref UnsafeUtilityEx.AsRef<T>(dataPtr), builders);
        protected virtual void Build(ref T data, IList<ITreeNode<INodeDataBuilder>> builders) {}
        public virtual INodeDataBuilder Self => this;
        public abstract IEnumerable<INodeDataBuilder> Children { get; }
    }

    public class BTVirtualRealSelf : INodeDataBuilder
    {
        private readonly INodeDataBuilder _self;
        public BTVirtualRealSelf(INodeDataBuilder self) => _self = self;
        public BehaviorNodeType NodeType => _self.NodeType;
        public int NodeId => _self.NodeId;
        public int Size => _self.Size;
        public unsafe void Build(void* dataPtr, ITreeNode<INodeDataBuilder>[] builders) {}
        public INodeDataBuilder Self => _self;
        public IEnumerable<INodeDataBuilder> Children => _self.Children;
    }
    
    public class BTVirtualDecorator<T> : BTVirtualNode<T> where T : struct, INodeData
    {
        private readonly INodeDataBuilder _child;
        public BTVirtualDecorator(INodeDataBuilder child) => _child = new BTVirtualRealSelf(child);
        public override IEnumerable<INodeDataBuilder> Children => _child.Yield();
    }
}
