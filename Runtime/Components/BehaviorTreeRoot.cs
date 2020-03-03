using System;
using System.Reflection;
using EntitiesBT.Entities;
using Unity.Entities;
using UnityEngine;

namespace EntitiesBT.Components
{
    [DisallowMultipleComponent]
    public class BehaviorTreeRoot : MonoBehaviour, IConvertGameObjectToEntity
    {
        [Flags]
        private enum AutoCreateType
        {
            BehaviorTreeComponent = 1 << 0
          , ReadOnly = 1 << 1
          , ReadWrite = 1 << 2
        }
        
        [Header("Order: File > RootNode")]
        [SerializeField] private TextAsset _file = default;
        [SerializeField] private BTNode _rootNode;
        [SerializeField] private BehaviorTreeThread _thread = BehaviorTreeThread.ForceRunOnMainThread;
        
        [Tooltip("add queried components of behavior tree into entity automatically")]
        [SerializeField] private AutoCreateType _autoCreateTypes = AutoCreateType.BehaviorTreeComponent;
        
        private void Reset()
        {
            _rootNode = GetComponentInChildren<BTNode>();
        }

        private bool HasFlag(AutoCreateType flag)
        {
            return (_autoCreateTypes & flag) == flag;
        }

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            if (_rootNode != null && _rootNode.GetComponent<StopConvertToEntity>() == null)
                _rootNode.gameObject.AddComponent<StopConvertToEntity>();
            
            var blob = _file != null ? _file.ToBlob() : _rootNode.ToBlob();
            var blobRef = new NodeBlobRef {BlobRef = blob};
            entity.AddBehaviorTree(dstManager, blobRef, _thread);

            if (_autoCreateTypes > 0)
            {
                var accessTypes = dstManager.HasComponent<BlackboardDataQuery>(entity)
                    ? dstManager.GetSharedComponentData<BlackboardDataQuery>(entity).Value
                    : blobRef.GetAccessTypes()
                ;

                foreach (var componentType in accessTypes)
                {
                    if (dstManager.HasComponent(entity, componentType)) continue;
                    var shouldCreate =
                        HasFlag(AutoCreateType.ReadOnly) && componentType.AccessModeType == ComponentType.AccessMode.ReadOnly
                        || HasFlag(AutoCreateType.ReadWrite) && componentType.AccessModeType == ComponentType.AccessMode.ReadWrite
                        || HasFlag(AutoCreateType.BehaviorTreeComponent) && TypeManager.GetType(componentType.TypeIndex)?.GetCustomAttribute<BehaviorTreeComponentAttribute>() != null
                    ;
                    if (shouldCreate) dstManager.AddComponent(entity, componentType);
                }
            }
        }
    }
}
