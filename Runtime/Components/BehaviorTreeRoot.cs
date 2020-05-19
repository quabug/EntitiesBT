using System;
using System.Reflection;
using EntitiesBT.Entities;
using Unity.Entities;
using UnityEngine;

namespace EntitiesBT.Components
{
    public class BehaviorTreeRoot : MonoBehaviour, IConvertGameObjectToEntity
    {
        [Header("Order: File > RootNode")]
        [SerializeField] private TextAsset _file = default;
        [SerializeField] private BTNode _rootNode;
        [SerializeField] private BehaviorTreeThread _thread = BehaviorTreeThread.ForceRunOnMainThread;
        
        [Tooltip("add queried components of behavior tree into entity automatically")]
        [SerializeField] private AutoCreateType _autoCreateTypes = AutoCreateType.BehaviorTreeComponent;

        [SerializeField] private int _order = 0;
        [SerializeField] private string _debugName = default;

        private void OnEnable() {}

        private void Reset()
        {
            _rootNode = GetComponentInChildren<BTNode>();
        }

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            if (!enabled) return;
            
            if (_rootNode != null && _rootNode.GetComponent<StopConvertToEntity>() == null)
                _rootNode.gameObject.AddComponent<StopConvertToEntity>();
            
            var blob = _file != null ? _file.ToBlob() : _rootNode.ToBlob();
            var blobRef = new NodeBlobRef {BlobRef = blob};
            entity.AddBehaviorTree(dstManager, blobRef, _order, _autoCreateTypes, _thread, _debugName);
        }
    }
}
