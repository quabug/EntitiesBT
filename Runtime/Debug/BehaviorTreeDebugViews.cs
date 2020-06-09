using System;
using EntitiesBT.Core;
using EntitiesBT.Entities;
using Unity.Entities;
using UnityEngine;

namespace EntitiesBT.DebugView
{
    [AddComponentMenu("")] // hide from component menu
    public class BTDebugView : MonoBehaviour
    {
        [NonSerialized] public EntityBlackboard? Blackboard;
        [NonSerialized] public int Index;
        [NonSerialized] public NodeBlobRef Blob;
        
        public bool IsValid => Blackboard != null && Blob.BlobRef.IsCreated;
        public EntityManager EntityManager => Blackboard.Value.EntityManager;
        public Entity Entity => Blackboard.Value.Entity;

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
            var blob = Blob;
            RuntimeData = blob.GetNodeData<T, NodeBlobRef>(Index);
            DefaultData = blob.GetNodeDefaultData<T, NodeBlobRef>(Index);
        }

        public override void Tick()
        {
            var blob = Blob;
            RuntimeData = blob.GetNodeData<T, NodeBlobRef>(Index);
        }

        protected virtual void OnValidate()
        {
            if (!IsValid) return;
            var blob = Blob;
            blob.GetNodeData<T, NodeBlobRef>(Index) = RuntimeData;
            blob.GetNodeDefaultData<T, NodeBlobRef>(Index) = DefaultData;
            Debug.Log("node data changed");
        }
    }
    
}
