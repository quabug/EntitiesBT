using Unity.Entities;
using UnityEngine;
using UnityEngine.InputSystem;

namespace EntitiesBT.Extensions.InputSystem
{
    public class InputActionAssetComponent : IComponentData
    {
        public InputActionAsset Value { get; internal set; }
    }
    
    public class InputActionAssetProxy : MonoBehaviour, IConvertGameObjectToEntity
    {
        public InputActionAsset InputActionAsset;
        
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new InputActionAssetComponent {Value = InputActionAsset});
        }
    }
}
