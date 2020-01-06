using Unity.Entities;
using UnityEngine;

namespace EntitiesBT.Components
{
    [DisallowMultipleComponent]
    public class BehaviorTreeFileRoot : MonoBehaviour, IConvertGameObjectToEntity
    {
        [SerializeField] private TextAsset _file = default;
        [SerializeField] private BehaviorTreeThread _thread = BehaviorTreeThread.ForceRunOnMainThread;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            entity.AddBehaviorTree(dstManager, _file.ToBlob(), _thread);
        }
    }
}
