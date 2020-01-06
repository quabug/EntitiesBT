using Entities;
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
        [SerializeField] private bool EnableJob = false;
        
        private void Reset()
        {
            RootNode = GetComponentInChildren<BTNode>();
        }

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            var blobRef = new NodeBlobRef(RootNode.ToBlob());
            if (EnableJob)
            {
                var dataQuery = new BlackboardDataQuery {Value = blobRef.BlobRef.GetAccessTypes()};
                var jobBlackboard = new EntityJobChunkBlackboard();
                VirtualMachine.Reset(blobRef, jobBlackboard);
                dstManager.AddComponentData(entity, new JobBlackboard { Value = jobBlackboard });
                dstManager.AddSharedComponentData(entity, dataQuery);
            }
            else
            {
                var mainThreadBlackboard = new EntityBlackboard(dstManager, entity);
                VirtualMachine.Reset(blobRef, mainThreadBlackboard);
                dstManager.AddComponentData(entity, new MainThreadOnlyBlackboard {Value = mainThreadBlackboard});
            }
            dstManager.AddComponentData(entity, blobRef);
            dstManager.AddComponentData(entity, new TickDeltaTime());
        }
    }
}
