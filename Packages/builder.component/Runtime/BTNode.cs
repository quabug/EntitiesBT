using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Blob;
using EntitiesBT.Core;
using EntitiesBT.Entities;
using EntitiesBT.Nodes;
using JetBrains.Annotations;
using UnityEngine;

namespace EntitiesBT.Components
{
    [DisallowMultipleComponent, ExecuteInEditMode]
    public abstract class BTNode : MonoBehaviour
    {
        public class Builder : INodeDataBuilder
        {
            [NotNull] private readonly BTNode _node;

            public int NodeId => _node.NodeId;

            public int NodeIndex { get; set; } = -1;

            public IBuilder BlobStreamBuilder => _node.BlobStreamBuilder;

            public IEnumerable<INodeDataBuilder> Children => _node.Children()
                .Where(child => child.IsValid)
                .Select(child => child.Node)
            ;

            public Builder([NotNull] BTNode node)
            {
                _node = node;
            }
        }

        public BehaviorNodeType BehaviorNodeType => NodeType.GetBehaviorNodeAttribute().Type;
        public int NodeId => NodeType.GetBehaviorNodeAttribute().Id;
        protected virtual Type NodeType { get; } = typeof(ZeroNode);
        public abstract IBuilder BlobStreamBuilder { get; }
        public virtual bool IsValid => gameObject.activeInHierarchy;
        public virtual INodeDataBuilder Node => new Builder(this);

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
            using (var file = new FileStream(path, FileMode.OpenOrCreate)) Node.SaveToStream(this.FindGlobalValuesList(), file);
            UnityEditor.AssetDatabase.Refresh();
        }
#endif
    }
    
    public abstract class BTNode<T> : BTNode where T : unmanaged, INodeData
    {
        protected override Type NodeType => typeof(T);
        public override IBuilder BlobStreamBuilder => new ValueBuilder<T>(_Value);
        protected virtual T _Value => default;
    }
}
