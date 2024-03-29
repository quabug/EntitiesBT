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
    public abstract class BTNode : MonoBehaviour, INodeDataBuilder
    {
        public BehaviorNodeType BehaviorNodeType => NodeType.GetBehaviorNodeAttribute().Type;
        public int NodeId => NodeType.GetBehaviorNodeAttribute().Id;
        protected virtual Type NodeType { get; } = typeof(ZeroNode);
        public int NodeIndex { get; set; } = 0;

        public virtual IEnumerable<INodeDataBuilder> Children => this.Children();
        public virtual object GetPreviewValue(string path) => throw new NotImplementedException();
        public virtual void SetPreviewValue(string path, object value) => throw new NotImplementedException();

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

            var activateChildCount = 0;
            foreach (Transform child in transform)
            {
                if (child.gameObject.activeSelf)
                {
                    if (activateChildCount < maxChildCount)
                    {
                        activateChildCount++;
                    }
                    else
                    {
                        Debug.LogError($"{BehaviorNodeType} node {name} is not allowed to have more than {maxChildCount} children", gameObject);
                        child.gameObject.SetActive(false);
                    }
                }
            }
#endif
        }

        protected virtual void OnValidate() {}

#if UNITY_EDITOR
        [ContextMenu("Save to file")]
        public void SaveToFile()
        {
            var path = UnityEditor.AssetDatabase.GetAssetPath(gameObject);
            path = string.IsNullOrEmpty(path) ? Application.dataPath : Path.GetDirectoryName(path);
            path = UnityEditor.EditorUtility.SaveFilePanel("save path", path, name, "bytes");
            if (string.IsNullOrEmpty(path)) return;
            using (var file = new FileStream(path, FileMode.OpenOrCreate)) this.SaveToStream(this.FindScopeValuesList(), file);
            UnityEditor.AssetDatabase.Refresh();
        }
#endif
    }
    
    public abstract class BTNode<T> : BTNode where T : unmanaged, INodeData
    {
        protected override Type NodeType => typeof(T);

        protected override unsafe void Build(void* dataPtr, BlobBuilder builder, ITreeNode<INodeDataBuilder>[] tree)
        {
            Build(ref UnsafeUtility.AsRef<T>(dataPtr), builder, tree);
        }
        
        protected virtual void Build(ref T data, BlobBuilder builder, ITreeNode<INodeDataBuilder>[] tree) {}
    }
}
