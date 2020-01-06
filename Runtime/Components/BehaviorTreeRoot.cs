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
            entity.AddBehaviorTree(dstManager, RootNode.ToBlob(), EnableJob);
        }
    }
}
