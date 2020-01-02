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
            VirtualMachine.Reset(blobRef, null);
            dstManager.AddComponentData(entity, blobRef);
            dstManager.AddComponentData(entity, new JobBehaviorTreeTag());
            dstManager.AddComponentData(entity, new TickDeltaTime());
            dstManager.AddSharedComponentData(entity, dataQuery);
        }
    }
}
