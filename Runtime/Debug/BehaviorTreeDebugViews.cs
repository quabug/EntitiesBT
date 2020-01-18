using System;
using System.Linq;
using EntitiesBT.Core;
using EntitiesBT.Entities;
using EntitiesBT.Nodes;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using UnityEngine;

namespace EntitiesBT.DebugView
{
    [AddComponentMenu("")] // hide from component menu
    public class BTDebugView : MonoBehaviour
    {
        public bool IsValid => EntityManager != null && Blackboard != null && Blob.BlobRef.IsCreated;
        
        [NonSerialized] public EntityManager EntityManager;
        [NonSerialized] public Entity Entity;
        [NonSerialized] public EntityBlackboard Blackboard;
        [NonSerialized] public NodeBlobRef Blob;
        [NonSerialized] public int Index;

        // TODO: not implemented yet
        [NonSerialized] public bool PauseOnTick = false;
        
        public virtual void Init() {}
        public virtual void Tick() {}
    }
    
    [BehaviorTreeDebugViewGeneric]
    public class BTDebugView<T> : BTDebugView where T : struct, INodeData
    {
        public T DefaultData;
        public T RuntimeData;

        public override void Init()
        {
            RuntimeData = Blob.GetNodeData<T>(Index);
            DefaultData = Blob.GetNodeDefaultData<T>(Index);
        }

        public override void Tick()
        {
            var dataSize = Blob.GetNodeDataSize(Index);
            var typeSize = UnsafeUtility.SizeOf<T>();
            if (dataSize != typeSize)
            {
                Debug.LogWarning($"Data size not match: data-{Index}({dataSize}) != {typeof(T).Name}({typeSize})");
                return;
            }
            RuntimeData = Blob.GetNodeData<T>(Index);
        }

        protected virtual void OnValidate()
        {
            if (!IsValid) return;
            Blob.GetNodeData<T>(Index) = RuntimeData;
            Blob.GetNodeDefaultData<T>(Index) = DefaultData;
            Debug.Log("node data changed");
        }
    }
    
}
