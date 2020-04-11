using System;
using System.Collections.Generic;
using System.IO;
using EntitiesBT.Core;
using EntitiesBT.Entities;
using EntitiesBT.Nodes;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using Unity.Entities;

namespace EntitiesBT.Components
{
    [DisallowMultipleComponent, ExecuteInEditMode]
#if ODIN_INSPECTOR
    public abstract class BTNode : Sirenix.OdinInspector.SerializedMonoBehaviour, INodeDataBuilder
#else
    public abstract class BTNode : MonoBehaviour, INodeDataBuilder
#endif
    {
        public BehaviorNodeType BehaviorNodeType => NodeType.GetBehaviorNodeAttribute().Type;
        public int NodeId => NodeType.GetBehaviorNodeAttribute().Id;
        protected virtual Type NodeType { get; } = typeof(ZeroNode);
        
        public virtual IEnumerable<INodeDataBuilder> Children => this.Children();
        public INodeDataBuilder Self => gameObject.activeSelf ? SelfImpl : null;

        protected virtual INodeDataBuilder SelfImpl => this;

        public unsafe BlobAssetReference Build(ITreeNode<INodeDataBuilder>[] builders)
        {
            if (NodeType.IsZeroSizeStruct()) return BlobAssetReference.Null;
            var blobBuilder = new BlobBuilder(Allocator.Temp, UnsafeUtility.SizeOf(NodeType));
            try
            {
                var dataPtr = blobBuilder.ConstructRootPtrByType(NodeType);
                Build(dataPtr, blobBuilder, builders);
                return blobBuilder.CreateReferenceByType(NodeType);
            }
            finally
            {
                blobBuilder.Dispose();
            }
        }

        protected virtual unsafe void Build(void* dataPtr, BlobBuilder blobBuilder, ITreeNode<INodeDataBuilder>[] builders) {}

        protected virtual void Reset() => name = GetType().Name;

        protected virtual void Update()
        {
#if UNITY_EDITOR
            if (UnityEditor.EditorApplication.isPlaying) return;
            
            int maxChildCount;
            switch (BehaviorNodeType)
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
                Debug.LogError($"{BehaviorNodeType} node {name} is not allowed to have more than {maxChildCount} children", gameObject);
                for (var i = childCount - 1; i >= maxChildCount; i--) DestroyImmediate(transform.GetChild(i).gameObject);
            }
#endif
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
        protected override Type NodeType => typeof(T);

        protected override unsafe void Build(void* dataPtr, BlobBuilder builder, ITreeNode<INodeDataBuilder>[] tree)
        {
            Build(ref UnsafeUtilityEx.AsRef<T>(dataPtr), builder, tree);
        }
        
        protected virtual void Build(ref T data, BlobBuilder builder, ITreeNode<INodeDataBuilder>[] tree) {}
    }
}
