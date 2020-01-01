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
            var bb = new EntityMainThreadBlackboard(dstManager, entity);
            
            VirtualMachine.Reset(blobRef, bb);
            
            dstManager.AddComponentData(entity, blobRef);
            dstManager.AddComponentData(entity, new MainThreadOnlyBlackboard {Value = bb});
            dstManager.AddComponentData(entity, new TickDeltaTime());
        }
    }
}
