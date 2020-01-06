using Unity.Entities;
using UnityEngine;

namespace EntitiesBT.Components
{
    [DisallowMultipleComponent]
    public class BehaviorTreeFileRoot : MonoBehaviour, IConvertGameObjectToEntity
    {
        [SerializeField] private TextAsset File = default;
        [SerializeField] private bool EnableJob = false;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            entity.AddBehaviorTree(dstManager, File.ToBlob(), EnableJob);
        }
    }
}
