using EntitiesBT.Core;
using EntitiesBT.Editor;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace EntitiesBT.Sample
{
    public class EntityMove : BTNode
    {
        private BTRoot _root => GetComponentInParent<BTRoot>();
        public Vector3 Velocity;
        
        public override IBehaviorNode BehaviorNode => new EntityMoveNode(_root.EntityManager);
        public override unsafe int Size => sizeof(EntityMoveNode.Data);
        public override unsafe void Build(void* dataPtr)
        {
            var ptr = (EntityMoveNode.Data*) dataPtr;
            ptr->Entity = _root.Entity;
            ptr->Velocity = Velocity;
        }
    }
    
    public class EntityMoveNode : IBehaviorNode
    {
        private readonly EntityManager _entityManager;

        public struct Data : INodeData
        {
            public Entity Entity;
            public float3 Velocity;
        }

        public EntityMoveNode(EntityManager entityManager)
        {
            _entityManager = entityManager;
        }
        
        public void Reset(VirtualMachine vm, int index) {}

        public NodeState Tick(VirtualMachine vm, int index)
        {
            ref var data = ref vm.GetNodeData<Data>(index);
            var translation = _entityManager.GetComponentData<Translation>(data.Entity);
            translation.Value += data.Velocity * Time.deltaTime;
            _entityManager.SetComponentData(data.Entity, translation);
            return NodeState.Running;
        }
    }
}
