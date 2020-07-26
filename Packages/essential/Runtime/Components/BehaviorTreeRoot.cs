using System.Linq;
using EntitiesBT.Core;
using EntitiesBT.Entities;
using Unity.Entities;
using UnityEngine;

namespace EntitiesBT.Components
{
    public class BehaviorTreeRoot : MonoBehaviour, IConvertGameObjectToEntity
    {
#if !ODIN_INSPECTOR
        [SerializeReferenceButton]
#endif
        [SerializeReference] private IBehaviorTreeSource _source = default;
        [SerializeField] private BehaviorTreeThread _thread = BehaviorTreeThread.ForceRunOnMainThread;
        
        [Tooltip("add queried components of behavior tree into entity automatically")]
        [SerializeField] private AutoCreateType _autoCreateTypes = AutoCreateType.All;

        [SerializeField] private int _order = 0;
        [SerializeField] private string _debugName = default;

        private void OnEnable() {}

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            var blob = new NodeBlobRef(_source.GetBlobAsset());
            var bb = new EntityBlackboard { Entity = entity, EntityManager = dstManager };
            VirtualMachine.Reset(ref blob, ref bb);

            dstManager.AddBuffer<BehaviorTreeBufferElement>(entity);
            dstManager.AddComponent<CurrentBehaviorTreeComponent>(entity);

            var behaviorTreeEntity = conversionSystem.CreateAdditionalEntity(gameObject);

#if UNITY_EDITOR
            dstManager.SetName(behaviorTreeEntity, $"[BT]{dstManager.GetName(entity)}:{_order}.{_debugName}");
#endif
            var query = blob.GetAccessTypes();
            var dataQuery = new BlackboardDataQuery(query, components =>
                dstManager.CreateEntityQuery(components.ToArray()));
            dstManager.AddSharedComponentData(behaviorTreeEntity, dataQuery);

            dstManager.AddComponentData(behaviorTreeEntity, new BehaviorTreeComponent
            {
                Blob = blob, Thread = _thread, AutoCreation = _autoCreateTypes
            });
            dstManager.AddComponentData(behaviorTreeEntity, new BehaviorTreeTargetComponent {Value = entity});
            dstManager.AddComponentData(behaviorTreeEntity, new BehaviorTreeOrderComponent {Value = _order});
        }
    }
}
