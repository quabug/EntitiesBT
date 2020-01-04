using Entities;
using EntitiesBT.Components;
using EntitiesBT.Core;
using EntitiesBT.Entities;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace EntitiesBT.Sample
{
    [DisallowMultipleComponent]
    public class BTCustomRoot : MonoBehaviour, IConvertGameObjectToEntity
    {
        [SerializeField] private BTNode RootNode;
        
        private void Reset()
        {
            RootNode = GetComponentInChildren<BTNode>();
        }

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            var blobRef = new NodeBlobRef(RootNode.ToBlob());
            var dataQuery = new BlackboardDataQuery();
            dataQuery.AddReadWrite(typeof(TickDeltaTime));
            dataQuery.AddReadWrite(typeof(Translation));
            var bb = new EntityJobChunkBlackboard();
            VirtualMachine.Reset(blobRef, bb);
            dstManager.AddComponentData(entity, blobRef);
            dstManager.AddComponentData(entity, new JobBehaviorTreeTag());
            dstManager.AddComponentData(entity, new TickDeltaTime());
            dstManager.AddComponentData(entity, new JobBlackboard { Value = bb });
            dstManager.AddSharedComponentData(entity, dataQuery);
        }
    }
}
