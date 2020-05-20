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
        [NonSerialized] public EntityBlackboard Blackboard;
        [NonSerialized] public int Index;
        
        public bool IsValid => Blackboard != null && Blob.BlobRef.IsCreated;
        public EntityManager EntityManager => Blackboard.EntityManager;
        public Entity Entity => Blackboard.Entity;
        public NodeBlobRef Blob => Blackboard.GetDataRef<BehaviorTreeBufferElement>().NodeBlob;

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
