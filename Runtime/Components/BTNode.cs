using System;
using System.Linq;
using EntitiesBT.Core;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace EntitiesBT.Components
{
    [DisallowMultipleComponent, ExecuteInEditMode]
    public abstract class BTNode : MonoBehaviour
    {
        public abstract BehaviorNodeType NodeType { get; }
        public abstract int NodeId { get; }
        public abstract int Size { get; }
        public abstract unsafe void Build(void* dataPtr);
        
        public int Index { get; set; }
        private void Reset() => name = GetType().Name;

        public int ChildCount => gameObject.Children<BTNode>().Count();

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
#endif
    }
    
    public abstract class BTNode<T, U> : BTNode, INodeDataBuilder
        where U : struct, INodeData
    {
        public override BehaviorNodeType NodeType => typeof(T).GetBehaviorNodeAttribute().Type;
        public override int NodeId => typeof(T).GetBehaviorNodeAttribute().Id;
        public override int Size => UnsafeUtility.SizeOf<U>();
        public override unsafe void Build(void* dataPtr) {}
    }
    
    public abstract class BTNode<T> : BTNode, INodeDataBuilder
        where T : struct, INodeData
    {
        public override int Size => UnsafeUtility.SizeOf<T>();
        public override unsafe void Build(void* dataPtr) {}
    }
    
    public struct ZeroNodeData : INodeData {}
}
