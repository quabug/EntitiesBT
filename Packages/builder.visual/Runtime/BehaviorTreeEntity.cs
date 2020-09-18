using System.Linq;
using EntitiesBT.Components;
using EntitiesBT.Core;
using EntitiesBT.Entities;
using Unity.Entities;
using UnityEngine;

namespace EntitiesBT.Builder.Visual
{
    [RequiresEntityConversion]
    public class BehaviorTreeEntity : MonoBehaviour, IConvertGameObjectToEntity
    {
        [SerializeReference, SerializeReferenceButton] private IBehaviorTreeSource _source = default;
        [SerializeField] private BehaviorTreeThread _thread = BehaviorTreeThread.ForceRunOnMainThread;

        [Tooltip("add queried components of behavior tree into entity automatically")]
        [SerializeField] private AutoCreateType _autoCreateTypes = AutoCreateType.All;

        [SerializeField] private string _debugName = default;

        private void OnEnable() {}

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            var blob = new NodeBlobRef(_source.GetBlobAsset());
            var bb = new EntityBlackboard { Entity = entity, EntityManager = dstManager };
            VirtualMachine.Reset(ref blob, ref bb);

#if UNITY_EDITOR
            dstManager.SetName(entity, $"[BT]{_debugName}");
#endif
            var query = blob.GetAccessTypes();
            var dataQuery = new BlackboardDataQuery(query, components =>
                // HACK: workaround for different world/dstManager in subscenes.
                // dstManager.CreateEntityQuery(components.ToArray()));
                World.DefaultGameObjectInjectionWorld.EntityManager.CreateEntityQuery(components.ToArray()));
            dstManager.AddSharedComponentData(entity, dataQuery);

            dstManager.AddComponentData(entity, new BehaviorTreeComponent
            {
                Blob = blob, Thread = _thread, AutoCreation = _autoCreateTypes
            });
        }
    }
}