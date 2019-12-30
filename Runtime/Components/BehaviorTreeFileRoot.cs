using EntitiesBT.Core;
using EntitiesBT.Entities;
using Unity.Entities;
using UnityEngine;

namespace EntitiesBT.Components
{
    [DisallowMultipleComponent]
    public class BehaviorTreeFileRoot : MonoBehaviour, IConvertGameObjectToEntity
    {
        [SerializeField] private TextAsset File;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            var blobRef = new NodeBlobRef(File.ToBlob());
            var blackboard = new EntityBlackboard(dstManager, entity);
            VirtualMachine.Reset(blobRef, blackboard);
            
            dstManager.AddComponentData(entity, blobRef);
            dstManager.AddComponentData(entity, new BlackboardComponent {Value = blackboard});
        }
    }
}
