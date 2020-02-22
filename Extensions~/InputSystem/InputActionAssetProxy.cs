using Unity.Entities;
using UnityEngine;
using UnityEngine.InputSystem;

namespace EntitiesBT.Extensions.InputSystem
{
    public class InputActionAssetComponent : IComponentData
    {
        public InputActionAsset Value { get; set; }
    }
    
    public class InputActionAssetProxy : MonoBehaviour, IConvertGameObjectToEntity
    {
        [SerializeField] private InputActionAsset _inputActionAsset;
        
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new InputActionAssetComponent {Value = _inputActionAsset});
        }
    }
}
