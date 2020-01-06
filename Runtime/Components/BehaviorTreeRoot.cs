using Unity.Entities;
using UnityEngine;

namespace EntitiesBT.Components
{
    [DisallowMultipleComponent]
    public class BehaviorTreeRoot : MonoBehaviour, IConvertGameObjectToEntity
    {
        [SerializeField] private BTNode _rootNode;
        [SerializeField] private BehaviorTreeThread _thread = BehaviorTreeThread.ForceRunOnMainThread;
        
        private void Reset()
        {
            _rootNode = GetComponentInChildren<BTNode>();
        }

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            entity.AddBehaviorTree(dstManager, _rootNode.ToBlob(), _thread);
        }
    }
}
