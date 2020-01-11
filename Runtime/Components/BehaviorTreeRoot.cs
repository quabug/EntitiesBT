using EntitiesBT.Entities;
using Unity.Entities;
using UnityEngine;

namespace EntitiesBT.Components
{
    [DisallowMultipleComponent]
    public class BehaviorTreeRoot : MonoBehaviour, IConvertGameObjectToEntity
    {
        [Header("Order: File > RootNode")]
        [SerializeField] private TextAsset _file = default;
        [SerializeField] private BTNode _rootNode;
        [SerializeField] private BehaviorTreeThread _thread = BehaviorTreeThread.ForceRunOnMainThread;
        
        private void Reset()
        {
            _rootNode = GetComponentInChildren<BTNode>();
        }

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            if (_rootNode != null && _rootNode.GetComponent<StopConvertToEntity>() == null)
                _rootNode.gameObject.AddComponent<StopConvertToEntity>();
            
            var blob = _file != null ? _file.ToBlob() : _rootNode.ToBlob();
            entity.AddBehaviorTree(dstManager, blob, _thread);
        }
    }
}
