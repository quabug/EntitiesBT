using System;
using EntitiesBT.Core;
using EntitiesBT.Entities;
using Unity.Entities;
using UnityEngine;

namespace EntitiesBT.Components
{
    [DisallowMultipleComponent]
    public class BehaviorTreeRoot : MonoBehaviour, IConvertGameObjectToEntity
    {
        [SerializeField] private BTNode RootNode;
        private void Reset()
        {
            RootNode = GetComponentInChildren<BTNode>();
        }

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            var blobRef = new NodeBlobRef(RootNode.ToBlob());
            var blackboard = new EntityBlackboard(dstManager, entity);
            
            VirtualMachine.Reset(blobRef, blackboard);

            dstManager.AddComponentData(entity, new TickDeltaTime{Value = TimeSpan.Zero});
            dstManager.AddComponentData(entity, blobRef);
            dstManager.AddComponentData(entity, new BlackboardComponent {Value = blackboard});
        }
    }
}
