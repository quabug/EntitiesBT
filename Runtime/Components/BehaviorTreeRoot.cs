using EntitiesBT.Entities;
using Unity.Entities;
using UnityEngine;

namespace EntitiesBT.Components
{
    public class BehaviorTreeRoot : MonoBehaviour, IConvertGameObjectToEntity
    {
        [SerializeReference] private IBehaviorTreeSource _source = default;
        [SerializeField] private BehaviorTreeThread _thread = BehaviorTreeThread.ForceRunOnMainThread;
        
        [Tooltip("add queried components of behavior tree into entity automatically")]
        [SerializeField] private AutoCreateType _autoCreateTypes = AutoCreateType.All;

        [SerializeField] private int _order = 0;
        [SerializeField] private string _debugName = default;

        private void OnEnable() {}

        private void Reset()
        {
            var source = new BehaviorTreeSourceGameObject
            {
                Root = GetComponentInChildren<BTNode>()
              , AutoDestroy = true
            };
            _source = source;
        }

        public async void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            if (!enabled) return;
            
            var blob = await _source.GetBlobAsset();
            var blobRef = new NodeBlobRef(blob);
            entity.AddBehaviorTree(dstManager, blobRef, _order, _autoCreateTypes, _thread, _debugName);
        }
    }
}
