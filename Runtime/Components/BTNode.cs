using System;
using System.Linq;
using EntitiesBT.Core;
using EntitiesBT.Entities;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Entities.Serialization;
using UnityEngine;

namespace EntitiesBT.Components
{
    [DisallowMultipleComponent, ExecuteInEditMode]
    public abstract class BTNode : MonoBehaviour, INodeDataBuilder
    {
        public abstract BehaviorNodeType NodeType { get; }
        public abstract int NodeId { get; }
        public abstract int Size { get; }
        public abstract unsafe void Build(void* dataPtr);
        private void Reset() => name = GetType().Name;
        protected int ChildCount => this.Children().Count();

#if UNITY_EDITOR
        private void Update()
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

        [ContextMenu("Save to file")]
        public void SaveToFile()
        {
            var path = UnityEditor.EditorUtility.SaveFilePanel("save path", Application.dataPath, "behavior-tree", "bytes");
            if (string.IsNullOrEmpty(path))
                return;

            using (var blob = this.ToBlob(Allocator.Temp))
            {
                using (var writer = new StreamBinaryWriter(path))
                {
                    writer.Write(NodeBlob.VERSION);
                    writer.Write(blob);
                }
                UnityEditor.AssetDatabase.Refresh();
            }
        }
#endif
    }

    public abstract class BTNode<T> : BTNode
    {
        public override BehaviorNodeType NodeType => typeof(T).GetBehaviorNodeAttribute().Type;
        public override int NodeId => typeof(T).GetBehaviorNodeAttribute().Id;
        public override int Size => 0;
        public override unsafe void Build(void* dataPtr) {}
    }
    
    public abstract class BTNode<T, U> : BTNode
        where U : struct, INodeData
    {
        public override BehaviorNodeType NodeType => typeof(T).GetBehaviorNodeAttribute().Type;
        public override int NodeId => typeof(T).GetBehaviorNodeAttribute().Id;
        public override int Size => UnsafeUtility.SizeOf<U>();
        public override unsafe void Build(void* dataPtr) => Build(ref UnsafeUtilityEx.AsRef<U>(dataPtr));
        protected virtual void Build(ref U data) {}
    }
}
